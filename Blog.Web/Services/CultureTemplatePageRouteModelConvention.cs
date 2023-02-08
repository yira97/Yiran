using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Blog.Web.Services;

public class CultureTemplatePageRouteModelConvention : IPageRouteModelConvention
{
    public void Apply(PageRouteModel model)
    {
        var newSelectorModels = new List<SelectorModel>();

        foreach (var selector in model.Selectors)
        {
            if (selector.AttributeRouteModel == null) continue;

            newSelectorModels.Add(new SelectorModel
            {
                AttributeRouteModel = new AttributeRouteModel
                {
                    Order = -1,
                    Template =
                        AttributeRouteModel.CombineTemplates("{culture:culture?}",
                            selector.AttributeRouteModel.Template),
                }
            });
        }

        foreach (var m in newSelectorModels)
        {
            model.Selectors.Add(m);
        }
    }
}