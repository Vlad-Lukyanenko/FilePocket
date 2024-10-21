using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components;
using System.Diagnostics.CodeAnalysis;

namespace FilePocket.Admin.Components
{
    public class InputGuid : InputBase<Guid>
    {
        public ElementReference? Element { get; protected set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);
            builder.OpenElement(0, "input");
            builder.AddMultipleAttributes(1, AdditionalAttributes);
            builder.AddAttribute(2, "type", "text");
            builder.AddAttribute(3, "class", CssClass);
            builder.AddAttribute(4, "value", BindConverter.FormatValue(CurrentValueAsString));
            builder.AddAttribute(5, "oninput", EventCallback.Factory.CreateBinder<string?>(this, __value => CurrentValueAsString = __value, CurrentValueAsString));
            builder.AddElementReferenceCapture(6, (__value) => { Element = __value; });
            builder.CloseElement();
        }

        protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out Guid result, [NotNullWhen(false)] out string? validationErrorMessage)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                result = new Guid();
                validationErrorMessage = null;
                return true;
            }

            if (Guid.TryParse(value, out Guid guidValue))
            {
                result = guidValue;
                validationErrorMessage = null;
                return true;
            }
            else
            {
                result = new Guid();
                validationErrorMessage = "The value could not be parsed to a Guid.";
                return false;
            }
        }

        protected override string FormatValueAsString(Guid value)
        {
            return value.ToString() ?? string.Empty;
        }
    }
}
