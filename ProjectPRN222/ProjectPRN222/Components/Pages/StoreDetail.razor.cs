using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ProjectPRN222.Models;

namespace ProjectPRN222.Components.Pages
{
    public partial class StoreDetail
    {
   
    [Parameter] public int id { get; set; }

        Store? store;
        bool editing = false;
        Store editModel = new();

        protected override async Task OnInitializedAsync()
        {
            Svc.OnChange += HandleChange;
            await Svc.InitializeAsync();
        }

        protected override void OnParametersSet()
        {
            store = Svc.Find(id);
            if (store is null)
            {
                _ = LoadDirect();
            }
        }

        async Task LoadDirect()
        {
            var s = await Svc.GetByIdAsync(id);
            if (s is not null)
            {
                await Svc.RefreshAsync();
                store = Svc.Find(id);
                StateHasChanged();
            }
        }

        void HandleChange()
        {
            store = Svc.Find(id);
            InvokeAsync(StateHasChanged);
        }

        // Copy ID bằng clipboard API qua JSInterop
        async Task CopyId()
        {
            await JS.InvokeVoidAsync("navigator.clipboard.writeText", id.ToString());
            // có thể hiện toast ở đây nếu bạn có component thông báo
        }

        void StartEdit()
        {
            if (store is null) return;
            editModel = new Store
            {
                id = store.id,
                sellerId = store.sellerId,
                storeName = store.storeName,
                description = store.description,
                bannerImageURL = store.bannerImageURL
            };
            editing = true;
        }

        async Task Save()
        {
            await Svc.UpdateAsync(editModel);
            editing = false;
        }

        async Task Delete()
        {
            // tùy chọn: confirm
            // if (!await JS.InvokeAsync<bool>("confirm", $"Xóa store #{id}?")) return;
            await Svc.DeleteAsync(id);
            Nav.NavigateTo("/store");
        }

        public void Dispose() => Svc.OnChange -= HandleChange;
    
}
}
