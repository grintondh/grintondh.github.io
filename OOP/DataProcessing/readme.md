# DataProcessing.cs

### Table of Contents  
[Lưu ý](#lưu-ý)  
1. [Khởi tạo](#1-khởi-tạo)  
2. [Trả về List<string> rỗng](#new-2-trả-về-list-rỗng) 
3. [Nhập trường (field) - Import()](#3-nhập-trường-field---import)
4. [Nhập dữ liệu (data) - Import()](#4-nhập-dữ-liệu-data---import)
5. [Lấy dữ liệu có điều kiện - GetList](#new-5-lấy-dữ-liệu-có-điều-kiện---getlist)
6. [Lấy phần tử lớn nhất / nhỏ nhất theo điều kiện - GetMaxMin()](#new-6-lấy-phần-tử-lớn-nhất--nhỏ-nhất-theo-điều-kiện---getmaxmin)
7. [Lấy số phần tử dữ liệu - GetLength()](#7-lấy-số-phần-tử-dữ-liệu---getlength)
8. [Lấy Offset và Limit gần nhất - GetOffsetLimitNow()](#8-lấy-offset-và-limit-gần-nhất---getoffsetlimitnow)
9. [Thêm phần tử mới - AddNewElement()](#9-thêm-phần-tử-mới---addnewelement)
10. [Xóa toàn bộ phần tử - DeleteAllElements()](#10-xóa-toàn-bộ-phần-tử---deleteallelements)
11. [Xóa một phần tử trong danh sách - DeleteElementInRange()](#11-xóa-một-phần-tử-trong-danh-sách---deleteelementinrange)
12. [Sửa một phần tử trong danh sách - ChangeElementInRange()](#12-sửa-một-phần-tử-trong-danh-sách---changeelementinrange)
13. [Chuyển từ DataRow sang JObject - ConvertDataRowToJObject()](#13-chuyển-từ-datarow-sang-jobject---convertdatarowtojobject)
14. [Copy data sang một biến mới - CopyData()](#new-14-copy-data-sang-một-biến-mới---copydata)
---
  
### Lưu ý

KHÔNG XÓA / THÊM BẤT KỲ DỮ LIỆU GÌ ĐỐI VỚI DATATABLE ĐƯỢC TRẢ VỀ. VIỆC THÊM / BỚT SỬA ĐỀU PHẢI THỰC HIỆN QUA CÁC HÀM DƯỚI ĐÂY:

```
AddNewElement()
DeleteElementInRange()
UpdateElementsInRange() (Hàm bao gồm xóa và sửa)
```

ĐỐI VỚI VIỆC SỬA (SỬ DỤNG HÀM ```UpdateElementsInRange()``` HOẶC ```ChangeElementInRange()```), NGHIÊM CẤM THAY ĐỔI THỨ TỰ CÁC DỮ LIỆU (DÒNG).
  
---

### 1. Khởi tạo

Tạo một biến với class DataProcessing. Ví dụ:

```
private DataProcessing dp = new DataProcessing();
```

---
  
### [NEW] 2. Trả về List<string> rỗng
  
```
DataProcessing.emptyList
```
  
---
  
### 3. Nhập trường (field) - Import()

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
  
---

### 4. Nhập dữ liệu (data) - Import()
  
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
  
---
  
### [NEW] 5. Lấy dữ liệu có điều kiện - GetList
  
#### Cú pháp
  
```
<tên biến>.GetList(int _offset, int _limit, List<string> _query, List<string> _columns, string _sorts)

<tên biến>.GetList(int _offset, int _limit)

<tên biến>.GetList(int _offset, int _limit, string _sort)

<tên biến>.GetList(int _offset, int _limit, List<string> _query, string _sort)

<tên biến>.GetList(int _offset, int _limit, List<string> _query, List<string> _columns)
```
  
- offset: Lấy từ vị trí nào
- limit: Lấy báo nhiêu vị trí
- query: Gồm n điều kiện. List query gồm 2 * n phần tử, phần tử 2 * i (chẵn) là tên property, 2 * i + 1 (lẻ tương ứng) là giá trị điều kiện.
- columns: Tên các cột cần lấy.
- [NEW] sorts: Điều kiện sắp xếp, có dạng "<tên cột> asc/desc", cách nhau bởi dấu phẩy. Ví dụ: "id asc, name desc": Sắp xếp tăng dần theo id, nếu trùng id thì giảm dần theo name.

#### [NEW] Các trường hợp:
- [NEW] query hoặc columns là DataProcessing.emptyList: Không có điều kiện / Lấy toàn bộ cột.
- query hoặc columns là List<string> {"SAME"}: Lấy điều kiện / các cột tại lần gọi trước đó. Mặc định ban đầu là null.
- Các trường hợp khác: Lấy theo điều kiện / các cột.

- [NEW] Nếu giá trị điều kiện dạng: "CONTAIN x" (x là một string) thì bộ lọc sẽ trả về các data có chứa x. Ví dụ là "CONTAIN 10", các giá trị sau đều thỏa mãn: "100", "010", "1010",...
 
  
#### Trả về

- [NEW] DataTable chứa dữ liệu lấy ra. Nếu lấy lỗi hệ thống sẽ hiện hộp thoại báo lỗi. Bấm OK để thoát chương trình, Cancel để quay lại.

- Nếu query có lẻ phần tử thì sẽ báo lỗi.
  
- [NEW] Nếu columns không chứa hai cột "NotDelete" hoặc "delete" thì sẽ báo lỗi.
  
- [NEW] Nếu không tồn tại cột trong columns thì sẽ báo lỗi.
  
#### Ví dụ
  
```
  List<string> query = new List<string> { "Name", "CONTAIN Lý"};
  List<string> column = new List<string> { "Id", "Name", "SubArray" };
  string sorts = "Name asc, Id desc";
  
  DataTable dt = categoriesData.GetList(0, 10, query, column, sorts);
  /*
     => Bắt đầu từ 0, xuất ra tối đa 10 dữ liệu. Trong đó trường Name chứa xâu "Lý" (VD: "Lý dễ", "Lý vừa",...). 
     Dữ liệu trả về là 1 DataTable có các trường Id, Name, SubArray 
     và sắp xếp tăng dần theo tên, nếu cùng tên xếp giảm dần theo Id.
  */
```
  
#### Lưu ý: Hiệu chỉnh Offset - Limit
  
Hệ thống tự động hiệu chỉnh Offset, Limit nếu không phù hợp:
  
```
Limit = Math.Min(Math.Max(0, Limit), length);
Offset = Math.Min(Math.Max(0, Offset), length - Limit);
```
  
với length là số phần tử dữ liệu (rows - hàng).
  
Các hàm thêm / sửa / xóa dưới đây sau khi thực hiện chức năng chính thì đều sẽ gọi hàm này để lấy dữ liệu. Do đó, Limit và Offset sẽ bị điều chỉnh dựa trên bảng mới.
  
Nếu chuyển từ dữ liệu cũ sang dữ liệu mới, trừ khi gọi các hàm thêm / cập nhật / xóa ở dưới thì sẽ không lưu lại bất kỳ một thay đổi nào.
  
---
  
### [NEW] 6. Lấy phần tử lớn nhất / nhỏ nhất theo điều kiện - GetMaxMin()
  
#### Cú pháp
  
```
<tên biến>.GetMaxMin(int _offset, int _limit, List<string> _query, string _sort, string _maxMin)
```

- offset: Lấy từ vị trí nào
- limit: Lấy báo nhiêu vị trí
- query: Gồm n điều kiện. List query gồm 2 * n phần tử, phần tử 2 * i (chẵn) là tên property, 2 * i + 1 (lẻ tương ứng) là giá trị điều kiện.
- [NEW] sorts: Điều kiện sắp xếp, có dạng "<tên cột> asc/desc". Ví dụ: "id asc name desc": Sắp xếp tăng dần theo id, nếu trùng id thì giảm dần theo name.
- maxMin: "MAX" nếu lấy phần tử cuối cùng theo sorts, "MIN" nếu lấy phần tử đầu tiên theo sorts.
  
#### Trả về
  
- DataRow chứa dữ liệu tương ứng.
  
- null nếu maxMin không phải "MAX" hoặc "MIN".
  
#### Lưu ý
  
- Hàm này chẳng qua là rút gọn cho cái này thôi:
  
```
  DataTable _sortedDB = GetList(_offset, _limit, _query, _sort);

            if (_maxMin == "MAX")
                return _sortedDB.Rows[_sortedDB.Rows.Count - 1];
            else if (_maxMin == "MIN")
                return _sortedDB.Rows[0];
            else
                return null;
```

---
  
### 7. Lấy số phần tử dữ liệu - GetLength()
  
#### Cú pháp
  
```
<tên biến>.GetLength()
```
  
#### Trả về

Số nguyên chứa số lượng phần tử dữ liệu
  
---
  
### 8. Lấy Offset và Limit gần nhất - GetOffsetLimitNow()
 
#### Cú pháp

```
<tên biến>.GetOffsetLimitNow()
```

#### Trả về
  
Một Tuple<int,int> (giống pair<int,int>). Trong đó .Item1 là Offset, .Item2 là Limit.

---
  
### 9. Thêm phần tử mới - AddNewElement()
  
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
  
---
  
### 10. Xóa toàn bộ phần tử - DeleteAllElements()
  
#### Cú pháp
  
```
<tên biến>.DeleteAllElements()
```
  
#### Trả về

Một DataTable rỗng (sau khi Clear toàn bộ phần tử đã lưu).
  
### 11. Xóa một phần tử trong danh sách - DeleteElementInRange()
  
#### Cú pháp

```
<tên biến>.DeleteElementInRange(DataTable dataTable, int indexInTable)
```
  
- dataTable: DataTable đang quản lý dữ liệu show ra.
  
- indexInTable: Thứ tự (index) của phần tử trong bảng dataTable.
  
#### Trả về

- [NEW] Không có

- Hệ thống sẽ báo lỗi khi dataTable = null hoặc indexInTable >= Limit. Do đó cần luôn có giá trị mặc định tại đây.
  
- [NEW] Nếu tồn tại property NotDelete thì nếu NotDelete = true thì hệ thống hiện dialog báo vượt quyền.
  
- [NEW] Không có thông báo nếu thành công.
  
#### Lưu ý
  
- Hàm chỉ cập nhật các phần tử đã được gọi trước đó bởi hàm GetList()
  
- [NEW] Một phần tử chỉ bị xóa nếu thỏa mãn: Delete = true và (nếu tồn tại property này) NotDelete = false. Nếu không xóa thì sẽ cập nhật lại với giá trị mới nhất trong dataTable.

- [NEW] ĐỂ XÓA MỘT PHẦN TỬ, KHÔNG ĐƯỢC XÓA PHẦN TỬ TRONG DATATABLE ĐƯỢC TRẢ VỀ, BẮT BUỘC PHẢI SET DELETE = TRUE (KHÔNG SỬA NOTDELETE ĐỂ TRÁNH BỊ XÓA NHẦM).
  
---
  
### 12. Sửa một phần tử trong danh sách - ChangeElementInRange()
  
#### Cú pháp

```
<tên biến>.ChangeElementInRange(DataTable dataTable, int indexInTable)
```
  
- dataTable: DataTable đang quản lý dữ liệu show ra.
  
- indexInTable: Thứ tự (index) của phần tử trong bảng _dataTable tính từ 0 đến Limit - 1 (= Offset đến Offset + Limit - 1trong bảng đầy đủ).
  
#### Trả về
  
- [NEW] Không có.
  
- Hệ thống sẽ báo lỗi khi dataTable = null hoặc indexInTable >= Limit. Do đó cần luôn có giá trị mặc định tại đây.

- [NEW] Không có thông báo nếu thành công.
  
#### Lưu ý
  
- [NEW] Hàm chỉ cập nhật các phần tử đã được gọi trước đó bởi hàm GetList()
  
---
  
### 13. Chuyển từ DataRow sang JObject - ConvertDataRowToJObject()
  
#### Cú pháp
  
```
<tên biến>.ConvertDataRowToJObject(DataRow _dataRow)
```
  
#### Trả về
  
- JObject chuyển từ DataRow trên.
  
### 14. Xuất dữ liệu - Export()
  
#### Cú pháp
  
```
<tên biến>.Export()
```
  
#### Trả về
  
- JArray chứa dữ liệu cuối cùng.
  
- Hệ thống hiện dialog thành công sau khi thành công.
  
#### Lưu ý

- Trong lúc xuất dữ liệu thì hệ thống sẽ tự động xóa trường Delete.

- Sử dụng JArray này cho hàm ```ExportJsonContentInDefaultFolder()``` của JsonProcessing. Ví dụ: 
  
```
JsonProcessing.ExportJsonContentInDefaultFolder("data.json", data.Export());
```
  
- Toàn bộ các quá trình thêm / xóa / sửa đều không lưu lên file mà chỉ chỉnh sửa trên các mảng. Chỉ khi gọi hàm Export() rồi ExportJsonContentInDefaultFolder() thì dữ liệu mới được lưu vào file json.
  
- Sau khi Export hãy Import lại dữ liệu mới rồi GetList để đảm bảo dữ liệu hiển thị là mới nhất.
  
---
  
### [NEW] 14. Copy data sang một biến mới - CopyData()
  
#### Cú pháp
  
```
<biến cũ>.CopyData(<biến mới>)
```
  
#### Trả về
  
Không có

#### Lưu ý
  
Sử dụng để:
  
- Tránh ảnh hưởng đến Offset và Limit cũ.
