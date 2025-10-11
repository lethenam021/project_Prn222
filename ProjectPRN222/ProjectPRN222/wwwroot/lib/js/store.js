
        // ====== State & APIs (localStorage demo) ======
        const apiKey = 'stores_v1';
        const prodKey = 'products_v1';

        const els = {
          // list view
          listView: document.getElementById('listView'),
          tbody: document.getElementById('tbody'),
          q: document.getElementById('q'), sortBy: document.getElementById('sortBy'), sortDir: document.getElementById('sortDir'), pageSize: document.getElementById('pageSize'),
          pageInfo: document.getElementById('pageInfo'), prev: document.getElementById('prev'), next: document.getElementById('next'),
          // detail view
          detailView: document.getElementById('detailView'), pageTitle: document.getElementById('pageTitle'),
          dvStoreName: document.getElementById('dvStoreName'), dvStoreId: document.getElementById('dvStoreId'), dvSellerId: document.getElementById('dvSellerId'),
          dvDescription: document.getElementById('dvDescription'), dvBanner: document.getElementById('dvBanner'), dvBannerURL: document.getElementById('dvBannerURL'),
          btnBack: document.getElementById('btnBack'), btnEditStore: document.getElementById('btnEditStore'), btnDeleteStore: document.getElementById('btnDeleteStore'),
          // products
          ptbody: document.getElementById('ptbody'), ppageInfo: document.getElementById('ppageInfo'), pprev: document.getElementById('pprev'), pnext: document.getElementById('pnext'),
          pq: document.getElementById('pq'), psortBy: document.getElementById('psortBy'), psortDir: document.getElementById('psortDir'),
          btnAddProduct: document.getElementById('btnAddProduct'),
          // globals
          btnAdd: document.getElementById('btnAdd'), btnExport: document.getElementById('btnExport'), btnImport: document.getElementById('btnImport'), fileImport: document.getElementById('fileImport')
        };

        const state = { data: [], page:1, pageSize:10, sortBy:'storeName', sortDir:'asc', q:'', editing:null,
          // detail
          currentId: null,
          // product paging
          ppage:1, psize:10, psortBy:'name', psortDir:'asc', pq:'' , pediting:null
        };

        const StoreAPI = {
          list(){ return JSON.parse(localStorage.getItem(apiKey)||'[]'); },
          saveAll(rows){ localStorage.setItem(apiKey, JSON.stringify(rows)); },
          upsert(row){ const list=this.list(); if(row.id){const i=list.findIndex(x=>x.id===row.id); if(i>-1) list[i]=row; else list.push(row);} else {row.id = list.reduce((m,x)=>Math.max(m, x.id||0),0)+1; list.push(row);} this.saveAll(list); return row; },
          remove(id){ this.saveAll(this.list().filter(x=>x.id!==id)); }
        };

        const ProductAPI = {
          list(){ return JSON.parse(localStorage.getItem(prodKey)||'[]'); },
          saveAll(rows){ localStorage.setItem(prodKey, JSON.stringify(rows)); },
          forStore(storeId){ return this.list().filter(p=>p.storeId===storeId); },
          upsert(row){ const list=this.list(); if(row.id){const i=list.findIndex(p=>p.id===row.id); if(i>-1) list[i]=row; else list.push(row);} else {row.id = list.reduce((m,x)=>Math.max(m, x.id||0),0)+1; list.push(row);} this.saveAll(list); return row; },
          remove(id){ this.saveAll(this.list().filter(p=>p.id!==id)); },
          removeByStore(storeId){ this.saveAll(this.list().filter(p=>p.storeId!==storeId)); }
        };

        // Seed dữ liệu mẫu nếu trống
        (function seed(){
          if(StoreAPI.list().length===0){
            StoreAPI.saveAll([
              {id:1,sellerId:1001,storeName:'Tech Corner',description:'Thiết bị & phụ kiện số',bannerImageURL:'https://images.unsplash.com/photo-1518770660439-4636190af475?w=1200&auto=format&fit=crop&q=60'},
              {id:2,sellerId:1002,storeName:'Homey Living',description:'Đồ gia dụng tối giản',bannerImageURL:'https://images.unsplash.com/photo-1519710164239-da123dc03ef4?w=1200&auto=format&fit=crop&q=60'},
              {id:3,sellerId:1003,storeName:'BookNest',description:'Sách hay mỗi ngày',bannerImageURL:'https://images.unsplash.com/photo-1519681393784-d120267933ba?w=1200&auto=format&fit=crop&q=60'}
            ]);
          }
          if(ProductAPI.list().length===0){
            ProductAPI.saveAll([
              {id:1, storeId:1, name:'Tai nghe Bluetooth X1', price:490000, stock:25, imageURL:'https://images.unsplash.com/photo-1518441982120-29f227d8d4f9?w=1200&auto=format&fit=crop&q=60', description:'Tai nghe không dây, pin 20h'},
              {id:2, storeId:1, name:'Bàn phím cơ TKL', price:1290000, stock:10, imageURL:'https://images.unsplash.com/photo-1517336714731-489689fd1ca8?w=1200&auto=format&fit=crop&q=60', description:'Switch Brown, LED trắng'},
              {id:3, storeId:2, name:'Máy xay mini', price:399000, stock:35, imageURL:'https://images.unsplash.com/photo-1567013127542-490d757e51fc?w=1200&auto=format&fit=crop&q=60', description:'Nhỏ gọn cho gia đình'},
              {id:4, storeId:3, name:'Sách: Minimalism', price:99000, stock:120, imageURL:'https://images.unsplash.com/photo-1524578271613-d550eacf6090?w=1200&auto=format&fit=crop&q=60', description:'Sống tối giản'}
            ]);
          }
        })();

        // ================== LIST VIEW LOGIC ==================
        function normalize(s){ return (s||'').toString().toLowerCase(); }

        function filtered(){
          const q = normalize(state.q);
          let arr = [...state.data];
          if(q){
            arr = arr.filter(x => normalize(x.storeName).includes(q) || normalize(x.description).includes(q) || String(x.sellerId).includes(q) || String(x.id).includes(q));
          }
          arr.sort((a,b)=>{
            const dir = state.sortDir==='asc'?1:-1; const k = state.sortBy;
            const va = (a[k]??'').toString(); const vb = (b[k]??'').toString();
            return va.localeCompare(vb, undefined, {numeric:true}) * dir;
          });
          return arr;
        }

        function paginate(arr){
          const total = arr.length; const pages = Math.max(1, Math.ceil(total/state.pageSize));
          if(state.page>pages) state.page=pages; const start = (state.page-1)*state.pageSize; const items = arr.slice(start, start+state.pageSize);
          return {items,total,pages};
        }

        function renderList(){
          const arr = filtered();
          const {items,total,pages} = paginate(arr);
          els.pageInfo.textContent = `Trang ${state.page}/${pages} — Tổng ${total}`;
          els.prev.disabled = state.page<=1; els.next.disabled = state.page>=pages;

          els.tbody.innerHTML = items.map(x=>`
            <tr>
              <td>${x.id}</td>
              <td><span class="pill">${x.sellerId}</span></td>
              <td><strong>${x.storeName}</strong></td>
              <td>${x.description||''}</td>
              <td><img class="banner" src="${x.bannerImageURL||''}" alt="Banner" onerror="this.src='';this.alt=''" /></td>
              <td>
                <div style="display:flex; gap:6px">
                  <button class="btn" title="Xem chi tiết" aria-label="Xem chi tiết" onclick="goDetail(${x.id})">
                    <svg class="i" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.6"><circle cx="12" cy="12" r="9"/><path d="M2 12s3.5-6 10-6 10 6 10 6-3.5 6-10 6-10-6-10-6Z"/></svg>
                  </button>
                  <button class="btn" title="Sửa" aria-label="Sửa" onclick="edit(${x.id})"><svg class="i" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.6"><path d="M4 20h4l10-10-4-4L4 16v4z"/></svg></button>
                  <button class="btn danger" title="Xóa" aria-label="Xóa" onclick="del(${x.id})"><svg class="i" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.6"><path d="M4 7h16"/><path d="M9 7v12M15 7v12"/><path d="M6 7l1-3h10l1 3"/></svg></button>
                </div>
              </td>
            </tr>`).join('');
        }

        // ================== DETAIL VIEW LOGIC ==================
        function formatVND(n){ return (n||0).toLocaleString('vi-VN', {style:'currency', currency:'VND'}); }

        function goDetail(id){ location.hash = `#/store/${id}`; }
        window.goDetail = goDetail; // expose

        function loadDetail(id){
          const store = StoreAPI.list().find(s=>s.id===id); if(!store){ alert('Không tìm thấy cửa hàng'); return; }
          state.currentId = id;
          els.pageTitle.textContent = 'Chi tiết cửa hàng';
          els.listView.hidden = true; els.detailView.hidden = false;
          els.dvStoreName.textContent = store.storeName||'—';
          els.dvStoreId.textContent = `#${store.id}`; els.dvSellerId.textContent = store.sellerId; els.dvDescription.textContent = store.description||'—';
          els.dvBanner.src = store.bannerImageURL||''; els.dvBannerURL.href = store.bannerImageURL||'#'; els.dvBannerURL.textContent = store.bannerImageURL||'';
          renderProducts();
        }

        function leaveDetail(){
          els.pageTitle.textContent = 'Quản lý hồ sơ cửa hàng';
          els.detailView.hidden = true; els.listView.hidden = false; state.currentId = null;
        }

        // ===== Products render & paging
        function pFiltered(){
          const q = normalize(state.pq);
          let arr = ProductAPI.forStore(state.currentId);
          if(q){ arr = arr.filter(p=> normalize(p.name).includes(q) || String(p.id).includes(q)); }
          arr.sort((a,b)=>{
            const dir = state.psortDir==='asc'?1:-1; const k = state.psortBy;
            const va = (a[k]??'').toString(); const vb = (b[k]??'').toString();
            return va.localeCompare(vb, undefined, {numeric:true}) * dir;
          });
          return arr;
        }

        function pPaginate(arr){
          const total = arr.length; const pages = Math.max(1, Math.ceil(total/state.psize));
          if(state.ppage>pages) state.ppage=pages; const start=(state.ppage-1)*state.psize; const items=arr.slice(start, start+state.psize);
          return {items,total,pages};
        }

        function renderProducts(){
          const arr = pFiltered();
          const {items,total,pages} = pPaginate(arr);
          els.ppageInfo.textContent = `Trang ${state.ppage}/${pages} — Tổng ${total}`;
          els.pprev.disabled = state.ppage<=1; els.pnext.disabled = state.ppage>=pages;
          els.ptbody.innerHTML = items.map(p=>`
            <tr>
              <td>${p.id}</td>
              <td><strong>${p.name}</strong><div style="color:var(--muted); font-size:12px">${p.description||''}</div></td>
              <td>${formatVND(p.price)}</td>
              <td>${p.stock}</td>
              <td><img src="${p.imageURL||''}" alt="img" style="width:120px;height:60px;object-fit:cover;border:1px solid var(--line)" onerror="this.src='';this.alt=''"/></td>
              <td>
                <div style="display:flex; gap:6px">
                  <button class="btn" title="Sửa" onclick="pEdit(${p.id})"><svg class="i" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.6"><path d="M4 20h4l10-10-4-4L4 16v4z"/></svg></button>
                  <button class="btn danger" title="Xóa" onclick="pDel(${p.id})"><svg class="i" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.6"><path d="M4 7h16"/><path d="M9 7v12M15 7v12"/><path d="M6 7l1-3h10l1 3"/></svg></button>
                </div>
              </td>
            </tr>`).join('');
        }

        // ================== EVENT BINDINGS ==================
        function load(){ state.data = StoreAPI.list(); renderList(); }

        // Expose for inline onclick
        window.edit = function(id){
          const row = state.data.find(x=>x.id===id); if(!row) return;
          state.editing = id; document.getElementById('dlgTitle').textContent='Chỉnh sửa cửa hàng';
          document.getElementById('sellerId').value=row.sellerId||'';
          document.getElementById('storeName').value=row.storeName||'';
          document.getElementById('description').value=row.description||'';
          document.getElementById('bannerImageURL').value=row.bannerImageURL||'';
          document.getElementById('dlg').showModal();
        }
        window.del = function(id){ const row = state.data.find(x=>x.id===id); if(confirm(`Xóa cửa hàng "${row?.storeName||id}"?`)){ ProductAPI.removeByStore(id); StoreAPI.remove(id); load(); } }

        // List toolbar
        els.q.addEventListener('input', ()=>{ state.q = els.q.value; state.page=1; renderList(); });
        els.sortBy.addEventListener('change', ()=>{ state.sortBy = els.sortBy.value; renderList(); });
        els.sortDir.addEventListener('change', ()=>{ state.sortDir = els.sortDir.value; renderList(); });
        els.pageSize.addEventListener('change', ()=>{ state.pageSize = Number(els.pageSize.value); state.page=1; renderList(); });
        els.prev.addEventListener('click', ()=>{ if(state.page>1){ state.page--; renderList(); } });
        els.next.addEventListener('click', ()=>{ state.page++; renderList(); });

        // Add/Save store
        document.getElementById('btnAdd').addEventListener('click', ()=>{ state.editing = null; document.getElementById('form').reset(); document.getElementById('dlgTitle').textContent='Thêm cửa hàng'; document.getElementById('dlg').showModal(); });
        document.getElementById('form').addEventListener('submit', (e)=>{
          e.preventDefault();
          const payload = { id: state.editing||undefined, sellerId: Number(document.getElementById('sellerId').value), storeName: document.getElementById('storeName').value.trim(), description: document.getElementById('description').value.trim(), bannerImageURL: document.getElementById('bannerImageURL').value.trim() };
          if(!payload.storeName){ alert('Vui lòng nhập Tên cửa hàng'); return; }
          if(!Number.isFinite(payload.sellerId)){ alert('SellerId không hợp lệ'); return; }
          StoreAPI.upsert(payload); document.getElementById('dlg').close(); load(); if(state.currentId===payload.id){ loadDetail(payload.id); }
        });

        // Import / Export
        els.btnExport.addEventListener('click', ()=>{ const blob = new Blob([JSON.stringify(StoreAPI.list(), null, 2)], {type:'application/json'}); const url = URL.createObjectURL(blob); const a = Object.assign(document.createElement('a'), {href:url, download:'stores-export.json'}); document.body.appendChild(a); a.click(); a.remove(); URL.revokeObjectURL(url); });
        els.btnImport.addEventListener('click', ()=> els.fileImport.click());
        els.fileImport.addEventListener('change', async (e)=>{ const f = e.target.files[0]; if(!f) return; try{ const text = await f.text(); const arr = JSON.parse(text); if(!Array.isArray(arr)) throw new Error('File không đúng định dạng mảng JSON'); const clean = arr.map(x=>({ id:Number(x.id)||undefined, sellerId:Number(x.sellerId)||0, storeName:String(x.storeName||'').trim(), description:String(x.description||'').trim(), bannerImageURL:String(x.bannerImageURL||'').trim() })); StoreAPI.saveAll(clean); load(); }catch(err){ alert('Lỗi nhập dữ liệu: '+err.message); } finally{ e.target.value=''; } });

        // Detail actions
        els.btnBack.addEventListener('click', ()=>{ location.hash = '#/stores'; });
        els.btnEditStore.addEventListener('click', ()=>{ if(!state.currentId) return; const s = StoreAPI.list().find(x=>x.id===state.currentId); if(!s) return; state.editing = s.id; document.getElementById('sellerId').value=s.sellerId; document.getElementById('storeName').value=s.storeName; document.getElementById('description').value=s.description; document.getElementById('bannerImageURL').value=s.bannerImageURL; document.getElementById('dlgTitle').textContent='Chỉnh sửa cửa hàng'; document.getElementById('dlg').showModal(); });
        els.btnDeleteStore.addEventListener('click', ()=>{ if(!state.currentId) return; const s = StoreAPI.list().find(x=>x.id===state.currentId); if(confirm(`Xóa cửa hàng "${s?.storeName||state.currentId}" và toàn bộ sản phẩm?`)){ ProductAPI.removeByStore(state.currentId); StoreAPI.remove(state.currentId); location.hash = '#/stores'; load(); }});
        document.getElementById('dvCopyId')?.addEventListener('click', async ()=>{ if(!state.currentId) return; await navigator.clipboard.writeText(String(state.currentId)); alert('Đã copy ID cửa hàng'); });
        document.getElementById('dvOpenBanner')?.addEventListener('click', ()=>{ const href = els.dvBannerURL.href; if(href && href !== '#') window.open(href, '_blank'); });

        // Product add/edit/delete
        window.pEdit = function(id){ const p = ProductAPI.list().find(x=>x.id===id); if(!p) return; state.pediting = id; document.getElementById('pdlgTitle').textContent='Sửa sản phẩm'; document.getElementById('pname').value=p.name; document.getElementById('pprice').value=p.price; document.getElementById('pstock').value=p.stock; document.getElementById('pimg').value=p.imageURL||''; document.getElementById('pdesc').value=p.description||''; document.getElementById('pdlg').showModal(); }
        window.pDel = function(id){ const p = ProductAPI.list().find(x=>x.id===id); if(confirm(`Xóa sản phẩm "${p?.name||id}"?`)){ ProductAPI.remove(id); renderProducts(); } }
        els.btnAddProduct.addEventListener('click', ()=>{ state.pediting=null; document.getElementById('pform').reset(); document.getElementById('pdlgTitle').textContent='Thêm sản phẩm'; document.getElementById('pdlg').showModal(); });
        document.getElementById('pform').addEventListener('submit', (e)=>{
          e.preventDefault();
          if(!state.currentId){ alert('Thiếu storeId'); return; }
          const payload = { id: state.pediting||undefined, storeId: state.currentId, name: document.getElementById('pname').value.trim(), price: Number(document.getElementById('pprice').value), stock: Number(document.getElementById('pstock').value), imageURL: document.getElementById('pimg').value.trim(), description: document.getElementById('pdesc').value.trim() };
          if(!payload.name){ alert('Nhập tên sản phẩm'); return; }
          if(!(payload.price>=0)){ alert('Giá không hợp lệ'); return; }
          if(!(Number.isInteger(payload.stock) && payload.stock>=0)){ alert('Tồn kho không hợp lệ'); return; }
          ProductAPI.upsert(payload); document.getElementById('pdlg').close(); renderProducts();
        });

        // Product filtering & paging
        els.pq.addEventListener('input', ()=>{ state.pq = els.pq.value; state.ppage=1; renderProducts(); });
        els.psortBy.addEventListener('change', ()=>{ state.psortBy = els.psortBy.value; renderProducts(); });
        els.psortDir.addEventListener('change', ()=>{ state.psortDir = els.psortDir.value; renderProducts(); });
        els.pprev.addEventListener('click', ()=>{ if(state.ppage>1){ state.ppage--; renderProducts(); } });
        els.pnext.addEventListener('click', ()=>{ state.ppage++; renderProducts(); });

        // ============ Simple hash router ============
        function router(){
          const hash = location.hash || '#/stores';
          const m = hash.match(/^#\/store\/(\d+)$/);
          if(m){ loadDetail(Number(m[1])); }
          else { leaveDetail(); }
        }
        window.addEventListener('hashchange', router);

        // Init
        function init(){ state.data = StoreAPI.list(); renderList(); router(); }
        init();





const fileInput = document.getElementById('bannerFile');
const urlInput = document.getElementById('bannerImageURL');
const preview = document.querySelector('.preview');
const previewImg = document.getElementById('previewImg');
const caption = document.getElementById('previewCaption');
const closeBtn = document.getElementById('previewCloseBtn');

let objectURL = null; // lưu blob url để revoke khi bỏ ảnh

fileInput.addEventListener('change', () => {
    if (fileInput.files && fileInput.files[0]) {
        urlInput.value = ""; // ưu tiên file -> clear URL
        if (objectURL) { URL.revokeObjectURL(objectURL); objectURL = null; }
        const file = fileInput.files[0];
        objectURL = URL.createObjectURL(file);
        showPreview(objectURL, `Nguồn: file • ${file.name}`);
    }
});

urlInput.addEventListener('input', () => {
    if (urlInput.value.trim()) {
        // ưu tiên URL -> clear file
        if (objectURL) { URL.revokeObjectURL(objectURL); objectURL = null; }
        fileInput.value = "";
    }
});

urlInput.addEventListener('change', () => {
    const v = urlInput.value.trim();
    if (isLikelyImageURL(v)) showPreview(v, `Nguồn: URL • ${v}`);
});

closeBtn.addEventListener('click', clearPreview);

// (tuỳ chọn) Nhấn Esc để bỏ ảnh
document.addEventListener('keydown', (e) => {
    if (e.key === 'Escape' && !preview.hidden) clearPreview();
});

function isLikelyImageURL(u) {
    try {
        const exts = ['.jpg', '.jpeg', '.png', '.gif', '.webp', '.bmp', '.avif', '.svg'];
        const url = new URL(u);
        return exts.some(e => url.pathname.toLowerCase().endsWith(e));
    } catch { return false; }
}

function showPreview(src, text) {
    previewImg.src = src;
    caption.textContent = text;
    preview.hidden = false;
}

function clearPreview() {
    // Revoke blob nếu có
    if (objectURL) { URL.revokeObjectURL(objectURL); objectURL = null; }
    // Xoá dữ liệu & ẩn preview
    previewImg.src = "";
    caption.textContent = "";
    fileInput.value = "";
    urlInput.value = "";
    preview.hidden = true;
}