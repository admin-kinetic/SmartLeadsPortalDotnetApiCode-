using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace SmartLeadsPortalDotNetApi.Conventions
{
    public class KebabCaseActionModelConvention: IActionModelConvention
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
                        var kebabCaseActionName = ConvertToKebabCase(actionName);
                        selector.AttributeRouteModel.Template = template.Replace("[action]", kebabCaseActionName);
                    }
                }
            }
        }

        private string ConvertToKebabCase(string input)
        {
            return string.Concat(input.Select((x, i) => i > 0 && char.IsUpper(x) ? "-" + x.ToString().ToLower() : x.ToString().ToLower()));
        }
    }
}
