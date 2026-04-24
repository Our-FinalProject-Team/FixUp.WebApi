using FixUp.Service.Dto;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixUp.Service.Interfases
{
    public interface IAnalysisService
    {
        (int CategoryId, double Confidence) DetectCategoryId(string text);
        Dictionary<string, int> GetWordCounts(string text);

        Task<CategoryAnalystDto> AnalyzeRequestAsync(IFormFile image, string prompt);
    }
}
