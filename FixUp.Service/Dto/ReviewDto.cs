using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ReviewDto
{
    public int Id { get; set; }
    public int Stars { get; set; }
    public string Content { get; set; }
    public int ProfessionalId { get; set; }
    public int ClientId { get; set; }
}