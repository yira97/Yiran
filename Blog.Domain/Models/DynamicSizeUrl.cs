namespace Blog.Domain.Models;

public record DynamicSizeUrl(
    string Xs, 
    string Sm, 
    string Md, 
    string Lg, 
    string Xl,
    string XsCrop169 = null,
    string SmCrop169 = null,
    string MdCrop169 = null,
    string LgCrop169 = null,
    string XlCrop169 = null
    );