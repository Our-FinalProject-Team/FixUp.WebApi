using FixUp.Service.Dto;
using FixUp.Service.Interfaces;
using FixUp.Service.Interfases;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Mscc.GenerativeAI;
using Mscc.GenerativeAI.Types;

namespace FixUp.Service.Services
{
    public class AnalysisService : IAnalysisService
    {
        private readonly IConfiguration _config;

        public AnalysisService(IConfiguration config)
        {
            _config = config;
        }

        public Dictionary<string, int> GetWordCounts(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return new Dictionary<string, int>();
            char[] punctuation = text.Where(char.IsPunctuation).Distinct().ToArray();
            string cleanText = text.ToLower();
            foreach (char c in punctuation) cleanText = cleanText.Replace(c.ToString(), "");

            return cleanText.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries)
                            .Where(w => w.Length > 1)
                            .GroupBy(w => w)
                            .ToDictionary(g => g.Key, g => g.Count());
        }

        public async Task<CategoryAnalystDto> AnalyzeRequestAsync(IFormFile? image, string prompt)
        {
            // 1. ניתוח טקסט מקומי (Dictionary)
            var (textCategoryId, textConfidence) = DetectCategoryId(prompt);
            // 2. ניתוח AI (Gemini)
            int aiCategoryId = 0;
            if (image != null)
                aiCategoryId = await GetAiCategoryAsync(image, prompt);

            //הלוגיקה היא ש-AI תמיד מביא 35 נקודות
            //ניתוח טקסט על כל מילה 10 נקודות
            
            int finalId = 0;
            double confidence = 0;

            if (textCategoryId == aiCategoryId && textCategoryId != 0 && aiCategoryId!= 0)
            {
                finalId = textCategoryId;
                confidence = Math.Min(textConfidence + 35,99);
            }
            else if (textCategoryId != 0) 
            {
                finalId = textCategoryId;
                confidence = Math.Min(textConfidence,65);
            }
            else
            if(aiCategoryId != 0)
            {
                finalId = aiCategoryId;
                confidence = 35;
            }
            else
            {
                finalId = 0;
                confidence = 0;
            }

             return new CategoryAnalystDto { CategoryId = finalId, ConfidenceScore = confidence };
        }
       
      
        private async Task<int> GetAiCategoryAsync(IFormFile? image, string prompt)
        {
            var apiKey = _config["Gemini:ApiKey"];
            var googleAI = new GoogleAI(apiKey);
            var model = googleAI.GenerativeModel(model: Model.Gemini25Flash);

            string AIprompt = "You are an expert in fault classification and building maintenance." +
               " Analyze the attached image and return ONLY the most appropriate category number from the following list" +
               ":\r\n\r\n1 - Security: Cameras, Alarm, Intercom, Protection, Monitoring, Photography, CCTV.\r\n2 " +
               "- Air Conditioning (AC): Air conditioner, Cooling, Heating, Gas, Compressor, Filter, Central AC.\r\n3 " +
               "- Painting: Painting, Whitewashing, Walls, Paint repairs, Spackle, Putty.\r\n4" +
               " - Plumbing: Leak, Tap, Pipe, Water, Clog, Sewage, Sink, Toilet.\r\n5 " +
               "- Electricity: Short circuit, Panel, Fuse, Grounding, Socket, Switch.\r\n6 " +
               "- Maintenance (Handyman): Furniture, Repair, Hanging, Shelf, Assembly, Carpentry.\r\n7 " +
               "- Cleaning: Clean, Maid, Polish, Windows, Dust, Washing.\r\n8 " +
               "- Appliances: Fridge, Washing machine, Oven, Dryer, Dishwasher,Computer,Laptop, Microwave.\r\n9 " +
               "- Locksmith: Lock, Key, Break-in, Cylinder, Door, Safe.\r\n10" +
               " - Gardening: Garden, Grass, Plants, Irrigation, Pruning.\r\n11 -" +
               " Wall Cladding: Ceramics, Marble, Stone, Bricks, Flooring.\r\n12 - " +
               "Construction: Structure, Concrete, Renovation, Drywall, Contractor.\r\n\r\nFinal Instructions:\r\n-" +
               " Return ONLY the category number (e.g., 5).\r\n- If the category cannot be identified at all, return 0.\r\n-" +
               " Do NOT write any additional words, explanations, or punctuation beyond the number.";

            var request = new GenerateContentRequest(AIprompt);


            if (image !=null && image.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await image.CopyToAsync(memoryStream);
                var imageBytes = memoryStream.ToArray();
                string base64Image = Convert.ToBase64String(imageBytes);
                await request.AddMedia(base64Image, image.ContentType);

            }


            try
            {
                // שליחה ל-Gemini (יעבוד גם רק עם טקסט וגם עם תמונה)
                var response = await model.GenerateContent(request);

                if (response != null && !string.IsNullOrEmpty(response.Text))
                {
                    if (int.TryParse(response.Text.Trim(), out int result))
                    {
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                // לוג שגיאה כדי שלא "ייתקע" לך הקוד בלי שתדעי למה
                Console.WriteLine($"Gemini API Error: {ex.Message}");
            }

            return 0;
        }


        public (int CategoryId, double Confidence) DetectCategoryId(string text)
        {
            var messageWords = GetWordCounts(text);
            int totalMessageWords = messageWords.Sum(w => w.Value); // סך המילים בטקסט

            if (totalMessageWords == 0) return (0, 0);

            Dictionary<int, int> categoryScores = new Dictionary<int, int>();
            var categoriesSection = _config.GetSection("ProfessionalSettings:Categories");

            foreach (var category in categoriesSection.GetChildren())
            {
                int categoryId = int.Parse(category.Key);
                categoryScores[categoryId] = 0;
                var languages = new[] { "Hebrew", "English", "Russian", "Arabic" };

                foreach (var lang in languages)
                {
                    var keywords = category.GetSection(lang).Get<List<string>>();
                    if (keywords == null) continue;
                    foreach (var keyword in keywords)
                    {
                        if (messageWords.ContainsKey(keyword.ToLower()))
                            categoryScores[categoryId] += messageWords[keyword.ToLower()];
                    }
                }
            }

            var bestCategory = categoryScores.OrderByDescending(x => x.Value).FirstOrDefault();

            if (bestCategory.Value > 0)
            {              
                double confidence = bestCategory.Value * 15;

                confidence = Math.Min(confidence, 90);

                return (bestCategory.Key, confidence);
            }

            return (0, 0);
        }
    }
}