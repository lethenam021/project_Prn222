using ProjectPRN222.Models;

namespace ProjectPRN222.Components.Pages
{
    public partial class StoreList
    {   // UI state
        bool isLoading = false;
        string q = "";
        string sortBy = "storeName";
        string sortDir = "asc";
        int page = 1;
        int pageSize = 10;

        // edit dialog
        bool showEdit = false;
        Store editModel = new();

        void Go(int id) => Nav.NavigateTo($"/store/{id}");

        int TotalItems => FilterAndSort(Svc.Stores).Count();
        int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalItems / (double)pageSize));

        protected override async Task OnInitializedAsync()
        {
            Svc.OnChange += HandleChange;
            await WithLoading(async () =>
            {
                await Svc.InitializeAsync(); 
            });
        }

        void HandleChange() => InvokeAsync(StateHasChanged);

        IEnumerable<Store> FilterAndSort(IEnumerable<Store> src)
        {
            var qn = (q ?? "").Trim().ToLowerInvariant();
            var query = string.IsNullOrEmpty(qn)
                ? src
                : src.Where(x =>
                    (x.storeName ?? "").ToLower().Contains(qn) ||
                    (x.description ?? "").ToLower().Contains(qn) ||
                    (x.sellerId?.ToString() ?? "").Contains(qn) ||
                    x.id.ToString().Contains(qn));

            query = (sortBy, sortDir) switch
            {
                ("id", "asc") => query.OrderBy(x => x.id),
                ("id", "desc") => query.OrderByDescending(x => x.id),
                ("sellerId", "asc") => query.OrderBy(x => x.sellerId),
                ("sellerId", "desc") => query.OrderByDescending(x => x.sellerId),
                ("storeName", "desc") => query.OrderByDescending(x => x.storeName),
                _ => query.OrderBy(x => x.storeName),
            };

            return query;
        }

        IEnumerable<Store> PageOf(IEnumerable<Store> src)
            => src.Skip((page - 1) * pageSize).Take(pageSize);

        void Prev() { if (!isLoading && page > 1) page--; }
        void Next() { if (!isLoading && page < TotalPages) page++; }

        void OpenEdit(Store? s)
        {
            editModel = s is null
                ? new Store() 
                : new Store
                {
                    id = s.id,
                    sellerId = s.sellerId,
                    storeName = s.storeName,
                    description = s.description,
                    bannerImageURL = s.bannerImageURL
                };
            showEdit = true;
        }

        async Task Save()
        {
            await WithLoading(async () =>
            {
                if (editModel.id == 0)
                    await Svc.AddAsync(editModel);
                else
                    await Svc.UpdateAsync(editModel);

                showEdit = false;
                // Svc.OnChange sẽ làm UI tự cập nhật
            });
        }

        async Task ConfirmDelete(int id)
        {
            await WithLoading(async () =>
            {
                await Svc.DeleteAsync(id);
                if (page > TotalPages) page = TotalPages;
            });
        }

        async Task Reload()
        {
            // nếu service có hàm riêng để reload, thay bằng Svc.ReloadAsync()
            await WithLoading(async () => await Svc.InitializeAsync());
        }

        // Bản dùng Task
        private async Task WithLoading(Func<Task> work)
        {
            try
            {
                isLoading = true;
                StateHasChanged();
                await work();
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

        public void Dispose()
        {
            Svc.OnChange -= HandleChange;
        }
    }
}
