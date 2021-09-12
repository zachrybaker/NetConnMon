using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using NetConnMon.Domain;

namespace NetConnMon.Components.EntityForm
{
    public partial class EntityForm<BaseEntityType> : ComponentBase
        where BaseEntityType : BaseEntity
    {
        [Parameter]
        public BaseEntity model { get; set; }
        [Parameter]
        public Action LoadAction { get; set; }
        [Parameter]
        public Action SaveAction { get; set; }
        [Parameter]
        public EventCallback OnValidSubmit { get; set; }

        private bool loading = true;
        protected override Task OnParametersSetAsync()
        {
            LoadAction.BeginInvoke(x =>
            {
                loading = false;
                StateHasChanged();
            }, model);
            return Task.CompletedTask;

        }

        private async Task OnValidSubmitAsync(EditContext context)
        {
            await OnValidSubmit.InvokeAsync();
        }
    }
}
