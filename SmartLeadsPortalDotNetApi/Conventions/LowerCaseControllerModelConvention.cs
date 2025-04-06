using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace SmartLeadsPortalDotNetApi.Conventions
{
    public class LowerCaseControllerModelConvention: IControllerModelConvention
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
                        selector.AttributeRouteModel.Template = template.Replace("[controller]", controllerName.ToLower());
                    }
                }
            }
        }
    }
}
