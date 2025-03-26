using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace SmartLeadsPortalDotNetApi.Conventions
{
    public class KebabCaseControllerModelConvention: IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            foreach (var selector in controller.Selectors)
            {
                if (selector.AttributeRouteModel != null)
                {
                    var template = selector.AttributeRouteModel.Template;
                    if (template != null && template.Contains("[controller]"))
                    {
                        var controllerName = controller.ControllerName;
                        var kebabCaseControllerName = ConvertToKebabCase(controllerName);
                        selector.AttributeRouteModel.Template = template.Replace("[controller]", kebabCaseControllerName);
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
