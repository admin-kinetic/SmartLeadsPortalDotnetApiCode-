using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace SmartLeadsPortalDotNetApi.Conventions
{
    public class LowerCaseActionModelConvention: IActionModelConvention
    {
        public void Apply(ActionModel action)
        {
            foreach (var selector in action.Selectors)
            {
                if (selector.AttributeRouteModel != null)
                {
                    var template = selector.AttributeRouteModel.Template;
                    if (template != null && template.Contains("[action]"))
                    {
                        var actionName = action.ActionName;
                        selector.AttributeRouteModel.Template = template.Replace("[action]", actionName.ToLower());
                    }
                }
            }
        }
    }
}
