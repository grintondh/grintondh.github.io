# Hướng dẫn sử dụng file JsonProcessing.cs và DataProcessing.cs

## [JsonProcessing.cs - Tải tại đây](JsonProcessing.cs)

### 1. Khởi tạo

- Không cần khởi tạo.

### 2. Đọc file Json - ImportJsonContentInDefaultFolder()

#### Cú pháp

```
JsonProcessing.ImportJsonContentInDefaultFolder(string JsonPath, string sampleJsonWebPath, string sampleContent)
```

- JsonPath: Relative Path với thư mục gốc là thư mục chứa file đang chạy (thư mục thường sau khi Publish hoặc bin khi Debug). Nếu là file ngay trong thư mục thì chỉ cần nhập tên file.
- sampleJsonWebPath: Nếu file Json không tìm thấy theo đường dẫn thì hệ thống sẽ tự động tạo 1 file json mới có nội dung lấy từ webpath.
- sampleContent: Nếu không có kết nối mạng / bị lỗi khi lấy dữ liệu thì sẽ tạo 1 file json mới có nội dung là sampleContent. Nếu sampleContent = null thì hệ thống hiện ra dialog không hoàn thành.

#### Trả về

JArray chứa nội dung toàn bộ file json. Nếu thực hiện tạo file thành công thì hệ thống hiện ra 1 dialog hoàn thành tạo file.

#### Lưu ý

Sử dụng hàm Import() của DataProcessing để nhập dữ liệu JArray vào DataTable.

### 3. Xuất ra file Json - ExportJsonContentInDefaultFolder()

#### Cú pháp

```
JsonProcessing.ExportJsonContentInDefaultFolder(string JsonPath, JArray JsonData)
```

- JsonPath: Relative Path với thư mục gốc là thư mục chứa file đang chạy (thư mục thường sau khi Publish hoặc bin khi Debug). Nếu là file ngay trong thư mục thì chỉ cần nhập tên file.
- JsonData: Nội dung muốn xuất ra file json dưới dạng JArray.

#### Trả về

JArray chứa nội dung toàn bộ file json mới. Nếu không tồn tại file theo JsonPath thì sẽ tự động tạo file mới.

#### Lưu ý

Sử dụng hàm Export() của DataProcessing để xuất từ DataTable trả về dữ liệu JArray.

## [DataProcessing.cs - Tải tại đây](DataProcessing.cs)

### Lưu ý

KHÔNG XÓA / THÊM BẤT KỲ DỮ LIỆU GÌ ĐỐI VỚI DATATABLE ĐƯỢC TRẢ VỀ. VIỆC THÊM / BỚT SỬA ĐỀU PHẢI THỰC HIỆN QUA CÁC HÀM DƯỚI ĐÂY:

```
AddNewElement()
DeleteElementInRange()
UpdateElementsInRange() (Hàm bao gồm xóa và sửa. Hạn chế dùng)
```

ĐỐI VỚI VIỆC SỬA (SỬ DỤNG HÀM ```UpdateElementsInRange()``` HOẶC ```ChangeElementInRange()```), NGHIÊM CẤM THAY ĐỔI THỨ TỰ CÁC DỮ LIỆU (DÒNG).

### 1. Khởi tạo

Tạo một biến với class DataProcessing. Ví dụ:

```
private DataProcessing dp = new DataProcessing();
```

### 2. Nhập trường (field) - Import()

#### Cú pháp

```
<tên biến>.Import(List<string> columns, List<Type> columnsType)
```
  
- columns: Tên các trường (còn gọi là columns - cột)
- columnsType: Tên các loại dữ liệu tương ứng. Chú ý đặt trong typeof(...). Ví dụ: typeof(string), typeof(int), typeof(bool), typeof(List<...>),...

#### Trả về

Không. Nếu gặp lỗi hệ thống sẽ hiện dialog báo lỗi.
  
Dữ liệu luôn phải có trường NotDelete.
  
#### Lưu ý
  
Hệ thống sẽ tự động thêm trường Delete vào dữ liệu với giá trị mặc định là false.

### 3. Nhập dữ liệu (data) - Import()
  
#### Cú pháp

```
<tên biến>.Import(JArray jsonDataList)
```

- jsonDataList: Danh sách dữ liệu dưới dạng JArray.
  
#### Trả về

  Không. Nếu gặp lỗi hệ thống sẽ hiện dialog báo lỗi.
  
#### Lưu ý
  
JArray là kết quả từ hàm ```ImportJsonContentInDefaultFolder()``` của JsonProcessing.
  
Dữ liệu luôn phải có trường NotDelete.
  
### 3. Lấy dữ liệu - GetList
  
#### Cú pháp
  
```
<tên biến>.GetList(int offset, int limit)
```
  
- offset: Lấy từ vị trí nào
- limit: Lấy báo nhiêu vị trí
  
#### Trả về

DataTable chứa dữ liệu lấy ra. Nếu lấy lỗi hệ thống sẽ hiện hộp loại Dialog cho phép gọi lại hàm nếu bấm Retry.
  
#### Lưu ý: Hiệu chỉnh Offset - Limit
  
Hệ thống tự động hiệu chỉnh Offset, Limit nếu không phù hợp:
  
```
Limit = Math.Min(Math.Max(0, Limit), length);
Offset = Math.Min(Math.Max(0, Offset), length - Limit);
```
  
với length là số phần tử dữ liệu (rows - hàng).
  
Các hàm thêm / sửa / xóa dưới đây sau khi thực hiện chức năng chính thì đều sẽ gọi hàm này để lấy dữ liệu. Do đó, Limit và Offset sẽ bị điều chỉnh dựa trên bảng mới.
  
Nếu chuyển từ dữ liệu cũ sang dữ liệu mới, trừ khi gọi các hàm thêm / cập nhật / xóa ở dưới thì sẽ không lưu lại bất kỳ một thay đổi nào.

### [NEW] 4. Lấy dữ liệu có điều kiện - GetList
  
#### Cú pháp
  
```
<tên biến>.GetList(int _offset, int _limit, List<string> _query))
```
  
- offset: Lấy từ vị trí nào
- limit: Lấy báo nhiêu vị trí
- query: Gồm n điều kiện. List query gồm 2 * n phần tử, phần tử 2 * i (chẵn) là tên property, 2 * i + 1 (lẻ tương ứng) là giá trị điều kiện.
  
#### Trả về

DataTable chứa dữ liệu lấy ra. Nếu lấy lỗi hệ thống sẽ hiện hộp loại Dialog cho phép gọi lại hàm nếu bấm Retry.

Nếu query có lẻ phần tử thì sẽ báo lỗi. Nếu bấm Retry liên tục vẫn sẽ gặp lỗi.
  
#### Lưu ý: Hiệu chỉnh Offset - Limit
  
Hệ thống tự động hiệu chỉnh Offset, Limit nếu không phù hợp:
  
```
Limit = Math.Min(Math.Max(0, Limit), length);
Offset = Math.Min(Math.Max(0, Offset), length - Limit);
```
  
với length là số phần tử dữ liệu (rows - hàng).
  
Các hàm thêm / sửa / xóa dưới đây sau khi thực hiện chức năng chính thì đều sẽ gọi hàm này để lấy dữ liệu. Do đó, Limit và Offset sẽ bị điều chỉnh dựa trên bảng mới.
  
Nếu chuyển từ dữ liệu cũ sang dữ liệu mới, trừ khi gọi các hàm thêm / cập nhật / xóa ở dưới thì sẽ không lưu lại bất kỳ một thay đổi nào.
  
### 5. Lấy số phần tử dữ liệu - GetLength()
  
#### Cú pháp
  
```
<tên biến>.GetLength()
```
  
#### Trả về

Số nguyên chứa số lượng phần tử dữ liệu
  
### 6. Lấy Offset và Limit gần nhất - GetOffsetLimitNow()
  
#### Cú pháp

```
<tên biến>.GetOffsetLimitNow()
```

#### Trả về
  
Một Tuple<int,int> (giống pair<int,int>). Trong đó .Item1 là Offset, .Item2 là Limit.
  
### 7. Thêm phần tử mới - AddNewElement()
  
#### Cú pháp
  
```
<tên biến>.AddNewElement(JObject element)
```
  
- element: Chứa phần tử thêm vào
  
#### Trả về
  
DataTable có chứa phần tử mới, với Limit giữ nguyên như lần gọi kế trước, Offset sẽ về tối đa.
  
Hệ thống sẽ hiện Dialog thông báo kể cả thành công hay thất bại.
  
#### Lưu ý
 
Để có được JObject element, ta dùng lệnh ```JObject.FromObject(data)```, với data ở class dạng tùy chỉnh.
  
### 8. Xóa toàn bộ phần tử
  
#### Cú pháp
  
```
<tên biến>.DeleteAllElements()
```
  
#### Trả về

Một DataTable rỗng (sau khi Clear toàn bộ phần tử đã lưu).

### 9. Cập nhật toàn bộ phần tử trong danh sách (Hạn chế dùng hàm này) - UpdateElementsInRange()
  
#### Cú pháp

```
<tên biến>.UpdateElementsInRange(DataTable dataTable)
```
  
- dataTable: DataTable đang quản lý dữ liệu show ra.
  
#### Trả về
  
DataTable sau khi cập nhật các phần tử với Offset và Limit được giữ nguyên.
  
Hệ thống sẽ báo lỗi khi dataTable = null. Do đó cần luôn có giá trị mặc định tại đây.
  
Hệ thống hiện ra dialog thành công sau khi hoàn thành cập nhật.
  
#### Lưu ý
  
[NEW] Hàm chỉ cập nhật các phần tử đã được gọi trước đó bởi hàm GetList()
  
Hàm bao gồm Xóa / Chỉnh sửa các phần tử. Một phần tử chỉ bị xóa nếu thỏa mãn: NotDelete = false và Delete = true. Nếu không xóa thì sẽ cập nhật lại với giá trị mới nhất trong dataTable.
  
[NEW] ĐỂ XÓA MỘT PHẦN TỬ, KHÔNG ĐƯỢC XÓA PHẦN TỬ TRONG DATATABLE ĐƯỢC TRẢ VỀ, BẮT BUỘC PHẢI SET DELETE = TRUE (KHÔNG SỬA NOTDELETE ĐỂ TRÁNH BỊ XÓA NHẦM).

### 10. Xóa một phần tử trong danh sách - DeleteElementInRange()
  
#### Cú pháp

```
<tên biến>.DeleteElementInRange(DataTable dataTable, int indexInTable)
```
  
- dataTable: DataTable đang quản lý dữ liệu show ra.
  
- indexInTable: Thứ tự (index) của phần tử trong bảng dataTable.
  
#### Trả về

[NEW] Không có

Hệ thống sẽ báo lỗi khi dataTable = null hoặc indexInTable >= Limit. Do đó cần luôn có giá trị mặc định tại đây.
  
Nếu NotDelete = true thì hệ thống hiện dialog báo vượt quyền.
  
[NEW] Không có thông báo nếu thành công.
  
#### Lưu ý
  
Hàm chỉ cập nhật các phần tử đã được gọi trước đó bởi hàm GetList()
  
Một phần tử chỉ bị xóa nếu thỏa mãn: NotDelete = false và Delete = true. Nếu không xóa thì sẽ cập nhật lại với giá trị mới nhất trong dataTable.

[NEW] ĐỂ XÓA MỘT PHẦN TỬ, KHÔNG ĐƯỢC XÓA PHẦN TỬ TRONG DATATABLE ĐƯỢC TRẢ VỀ, BẮT BUỘC PHẢI SET DELETE = TRUE (KHÔNG SỬA NOTDELETE ĐỂ TRÁNH BỊ XÓA NHẦM).
  
### 11. Sửa một phần tử trong danh sách - ChangeElementInRange()
  
#### Cú pháp

```
<tên biến>.ChangeElementInRange(DataTable dataTable, int indexInTable)
```
  
- dataTable: DataTable đang quản lý dữ liệu show ra.
  
- indexInTable: Thứ tự (index) của phần tử trong bảng _dataTable tính từ 0 đến Limit - 1 (= Offset đến Offset + Limit - 1trong bảng đầy đủ).
  
#### Trả về
  
[NEW] Không có.
  
Hệ thống sẽ báo lỗi khi dataTable = null hoặc indexInTable >= Limit. Do đó cần luôn có giá trị mặc định tại đây.

[NEW] Không có thông báo nếu thành công.
  
#### Lưu ý
  
[NEW] Hàm chỉ cập nhật các phần tử đã được gọi trước đó bởi hàm GetList()
  
### 12. Xuất dữ liệu - Export()
  
#### Cú pháp
  
```
<tên biến>.Export()
```
  
#### Trả về
  
JArray chứa dữ liệu cuối cùng.
  
Hệ thống hiện dialog thành công sau khi thành công.
  
#### Lưu ý

Trong lúc xuất dữ liệu thì hệ thống sẽ tự động xóa trường Delete.

Sử dụng JArray này cho hàm ```ExportJsonContentInDefaultFolder()``` của JsonProcessing. Ví dụ: 
  
```
JsonProcessing.ExportJsonContentInDefaultFolder("data.json", data.Export());
```
  
Toàn bộ các quá trình thêm / xóa / sửa đều không lưu lên file mà chỉ chỉnh sửa trên các mảng. Chỉ khi gọi hàm Export() rồi ExportJsonContentInDefaultFolder() thì dữ liệu mới được lưu vào file json.
  
Sau khi Export hãy Import lại dữ liệu mới rồi GetList để đảm bảo dữ liệu hiển thị là mới nhất.
  
