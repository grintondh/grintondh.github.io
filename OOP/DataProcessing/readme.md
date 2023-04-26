# DataProcessing.cs

### Table of Contents  
[Lưu ý](#lưu-ý)  
1. [Khởi tạo](#1-khởi-tạo)  
2. [Trả về List string rỗng](#updated-2-trả-về-list-string-rỗng) 
3. [StatusCode](#updated-3-statuscode)
4. [Nhập dữ liệu (cột - columns) - Import()](#updated-4-nhập-dữ-liệu-cột---columns---import)
5. [Nhập dữ liệu (dòng - rows) - Import()](#updated-5-nhập-dữ-liệu-hàng---rows---import)
6. [Lấy dữ liệu có điều kiện - Get()](#update-6-lấy-dữ-liệu-có-điều-kiện---get)
7. [Lấy phần tử đầu tiên trả về theo điều kiện - GetFirstRow()](#new-7-lấy-phần-tử-đầu-tiên-trả-về-theo-điều-kiện---getfirstrow)
8. [Lấy số phần tử dữ liệu - Length()](#8-lấy-số-phần-tử-dữ-liệu---length)
9. [Xóa toàn bộ phần tử - DeleteAll()](#update-9-thêm-phần-tử-mới---insert)
10. [Xóa (các) phần tử thỏa mãn điều kiện - Delete()](#update-10-xóa-toàn-bộ-phần-tử---deleteall)
11. [Xóa một phần tử trong danh sách - DeleteElementInRange()](#update-11-xóa-các-phần-tử-thỏa-mãn-điều-kiện---delete)
12. [Cập nhật (các) phần tử thỏa mãn điều kiện - Update()](#update-12-cập-nhật-các-phần-tử-thỏa-mãn-điều-kiện---update)
13. [Chuyển từ DataRow sang JObject - ConvertDataRowToJObject()](#13-chuyển-từ-datarow-sang-jobject---convertdatarowtojobject)
14. [Xuất dữ liệu - Export()](#14-xuất-dữ-liệu---export)
15. [Quay lại bản dữ liệu trước đó - Undo()](#update-15-quay-lại-bản-dữ-liệu-trước-đó---undo)



---



### Lưu ý

KHÔNG XÓA / THÊM BẤT KỲ DỮ LIỆU GÌ ĐỐI VỚI DATATABLE ĐƯỢC TRẢ VỀ. VIỆC THÊM / BỚT SỬA ĐỀU PHẢI THỰC HIỆN QUA CÁC HÀM DƯỚI ĐÂY:

```
Insert();
DeleteAll();
Delete();
Update();
```

---

### 1. Khởi tạo

Tạo một biến với class DataProcessing. Ví dụ:

```
private DataProcessing dp = new();
```

---
  
### [UPDATED] 2. Trả về List string rỗng
  
```
DataProcessing.EmptyList
```
  
---

### [UPDATED] 3. StatusCode

```
DataProcessing.StatusCode.OK (= 1)
DataProcessing.StatusCode.Error (= 2)
DataProcessing.StatusCode.Unset (= 0)
```

- Chứa kết quả trả về của một số hàm ở dưới.
- Thông thường, nếu kết quả trả về là DataProcessing.StatusCode.OK tức hàm chạy thành công, DataProcessing.StatusCode.Error tức hàm chạy lỗi. Sử dụng kết quả này để "ném" Exception tránh gây lỗi dữ liệu.

#### Ví dụ

```
Categories _newCategory = new()
{
    ... // Khởi tạo biến _newCategory
};

// Nếu Insert (thêm phần tử) bị lỗi thì "ném" Exception
if (categoriesData.Insert(JObject.FromObject(_newCategory)) == DataProcessing.StatusCode.Error)
    throw new Exception();
```
  
---

### [UPDATED] 4. Nhập dữ liệu (cột - columns) - Import()

#### Cú pháp

```
<tên biến>.Import(List<string> columns, List<Type> columnsType, List<string> columnsKey)
```
  
- columns: Tên các trường (còn gọi là columns - cột)
- columnsType: Tên các loại dữ liệu tương ứng. Chú ý đặt trong typeof(...). Ví dụ: typeof(string), typeof(int), typeof(bool), typeof(List<...>),...
- [UPDATE] columnsKey: Khóa hằng số, nhận giá trị: "PRIMARY KEY" (UNIQUE + NOT NULL) - khóa chính, "UNIQUE" - các giá trị phải khác nhau, "NOT NULL" - các giá trị phải khác null

#### [UPDATE] Trả về

Một số nguyên. Trong đó

- Nếu columnsKey không chứa "PRIMARY KEY" nào cả, hệ thống sẽ báo lỗi và trả về StatusCode.Error
- Nếu 3 List không có số phần tử bằng nhau, hệ thống sẽ báo lỗi và trả về StatusCode.Error
- Còn lại thì hệ thống trả về StatusCode.OK
  
~~Dữ liệu luôn phải có trường NotDelete.~~
  
#### [UPDATE] Lưu ý

~~Hệ thống sẽ tự động thêm trường Delete vào dữ liệu với giá trị mặc định là false.~~
  
---

### [UPDATED] 5. Nhập dữ liệu (hàng - rows) - Import()
  
#### Cú pháp

```
<tên biến>.Import(JArray jsonDataList)
```

- jsonDataList: Danh sách dữ liệu đã xuất ra từ file Json (thông qua hàm bên JsonProcessing.cs)
  
#### [UPDATE] Trả về

Một số nguyên. Trong đó

- Nếu danh sách đầu vào (jsonDataList) rỗng (null), hệ thống sẽ báo lỗi và trả về StatusCode.Error
- Nếu gặp exception trong quá trình chuyển dữ liệu, hệ thống sẽ báo lỗi (có kèm thông tin exception) và trả về StatusCode.Error
- Còn lại thì hệ thống trả về StatusCode.OK
  
#### [UPDATE] Lưu ý
  
JArray là kết quả từ hàm ```ImportJsonContentInDefaultFolder()``` của JsonProcessing.
  
~~Dữ liệu luôn phải có trường NotDelete.~~

#### Ví dụ

```
private readonly DataProcessing categoriesData = new();
private readonly List<string> showColumns = new() { "Id", "Name", "SubArray", "QuestionArray", "Description", "IdNumber" };
private readonly List<Type> showType = new() { typeof(int), typeof(string), typeof(JArray), typeof(JArray), typeof(string), typeof(string) };
private readonly List<string> showKey = new() { "PRIMARY KEY", "UNIQUE", "", "", "", "UNIQUE"};
private DataTable? categoriesDataTable = new();
        
categoriesData.Import(showColumns, showType, showKey);
categoriesData.Import(_categoriesData);
categoriesDataTable = categoriesData.Offset(0).Limit(50).Get();
```
  
---
  
### [UPDATE] 6. Lấy dữ liệu có điều kiện - Get()
  
#### Cú pháp đầy đủ
  
```
<tên biến>.Init()           //            
          .Offset(offset)   // int        
          .Limit(limit)     // int        
          .Query(query)     // List<string>
          .Select(columns)  // List<string>
          .Sort(sorts)      // string      
          .Get()            //             
```

| Tên hàm | Cách gọi hàm | Bắt buộc (?) | Giá trị tham số | Giá trị mặc định (nếu không gọi) | Kiểu biến tham số | Ví dụ gọi hàm |
| -- | -- | -- | -- | -- | -- | -- |
| Khởi tạo tham số | Init() | Bắt buộc |  |  |  | ```.Init()``` |
| Offset | Offset(offset) | Không bắt buộc | Lấy từ vị trí nào | 0 | int | ```.Offset(20)``` |
| Limit | Limit(limit) | Không bắt buộc | Lấy báo nhiêu vị trí | DEFAULT_LIMIT (= 25) | int | ```.Limit(10)`` |
| Truy vấn | Query(query) | Không bắt buộc | Gồm n điều kiện. List query gồm 2 * n phần tử, phần tử 2 * i (chẵn) là tên property, 2 * i + 1 (lẻ tương ứng) là giá trị điều kiện. Đọc về "CONTAIN" ở phía dưới | EmptyList (không có điều kiện) | List<string> | ```List<string> query = new() { "Id", "CONTAIN 30" }; .Query(query)``` |
| Cột | Select(columns) | Không bắt buộc | Tên các cột cần lấy | EmptyList (lấy toàn bộ cột) | List<string> | ```List<string> select = new() { "Name", "Password" }; .Select(select)``` |
| Sắp xếp | Sort(sorts) | Không bắt buộc | Điều kiện sắp xếp, có dạng "<tên cột> asc/desc", cách nhau bởi dấu phẩy. Ví dụ: "id asc, name desc": Sắp xếp tăng dần theo id, nếu trùng id thì giảm dần theo name. | null (không có truy vấn sắp xếp) | string | ```DataRow? _maxIdRow = categoriesData.Offset(0).Limit(15).Sort("Id desc, Name asc").Get()``` |

#### [UPDATE] Lưu ý
- Nếu không gọi các hàm Offset, Limit, Query, Select, Sort thì giá trị tương ứng của chúng là giá trị mặc định (bảng trên).
- Nếu giá trị điều kiện (query) dạng: "CONTAIN x" (x là một string) thì bộ lọc sẽ trả về các data có chứa x. Ví dụ là "CONTAIN 10", các giá trị sau đều thỏa mãn: "100", "0 10", "1010",...
- Một giá trị thỏa mãn điều kiện nếu .ToString().ToLower() của chúng đều bằng nhau. (Nếu gặp lỗi phần này cần báo ngay).

  
#### [UPDATE] Trả về
- Nếu query hoặc columns là null thì hệ thống báo lỗi và trả về null.
- Nếu query có lẻ các phần tử thì hệ thống báo lỗi và trả về null.
- Nếu gặp lỗi trong khi duyệt dữ liệu thì hệ thống báo lỗi và trả về null.
- Nếu columns chứa cột không được Import ban đầu thì hệ thống báo lỗi và trả về null.
- Nếu gặp exception trong quá trình chuyển dữ liệu, hệ thống sẽ báo lỗi (có kèm thông tin exception) và trả về null.
- Các trường hợp còn lại trả về DataTable chứa dữ liệu cần lấy.
  
- Nếu không có cột nào trong columns có khóa hằng là PRIMARY KEY hoặc UNIQUE thì sẽ hiện thông báo cảnh báo.
  
#### Ví dụ
  
```
  List<string> query = new List<string> { "Name", "CONTAIN Lý"};
  List<string> column = new List<string> { "Id", "Name", "SubArray" };
  string sorts = "Name asc, Id desc";
  
  DataTable dt = categoriesData.Init().Offset(0).Limit(10).Query(query).Select(column).Get();
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
    
---
  
### [NEW] 7. Lấy phần tử đầu tiên trả về theo điều kiện - GetFirstRow()
  
#### Cú pháp đầy đủ
  
```
<tên biến>.Init()           //            
          .Offset(offset)   // int        
          .Limit(limit)     // int        
          .Query(query)     // List<string>
          .Select(columns)  // List<string>
          .Sort(sorts)      // string      
          .GetFirstRow()    //         
```

- Về các thành phần tương tự như Get()
  
#### Trả về
  
- Các điều kiện thông báo lỗi như phần 6.
- Ở trường hợp còn lại, trả về một DataRow chứa dữ liệu của hàng đầu tiên theo danh sách.

---
  
### 8. Lấy số phần tử dữ liệu - Length()
  
#### Cú pháp
  
```
<tên biến>.Length()
```
  
#### Trả về

Số nguyên chứa số lượng phần tử dữ liệu
  
---
    
### [UPDATE] 9. Thêm phần tử mới - Insert()
  
#### Cú pháp
  
```
<tên biến>.Insert(JObject element)
```
  
- element: Chứa phần tử thêm vào
  
#### [UPDATE] Trả về
  
Một số nguyên. Trong đó:
  
- Nếu element rỗng thì hệ thống báo lỗi và trả về StatusCode.Error
- Nếu tồn tại một cột PRIMARY KEY / NOT NULL mà trong element chứa giá trị null thì hệ thống báo lỗi và trả về StatusCode.Error
- Nếu tồn tại một cột PRIMARY KEY / UNIQUE mà trong element chứa giá trị trùng với các giá trị khác trong bảng thì hệ thống báo lỗi và trả về StatusCode.Error
- Nếu gặp exception trong quá trình chuyển dữ liệu, hệ thống sẽ báo lỗi (có kèm thông tin exception) và trả về StatusCode.Error
- Trong các trường hợp khác, hệ thống báo thành công và trả về StatusCode.OK
  
#### Lưu ý
 
Để có được JObject element, ta dùng lệnh ```JObject.FromObject(data)```, với data ở class dạng tùy chỉnh.
  
---
  
### [UPDATE] 10. Xóa toàn bộ phần tử - DeleteAll()
  
#### Cú pháp
  
```
<tên biến>.DeleteAll()
```

- Có tác dụng clear ListElement.
  
#### Trả về

- Không có.
  
---
  
### [UPDATE] 11. Xóa (các) phần tử thỏa mãn điều kiện - Delete()
  
#### Cú pháp

```
<tên biến>.Init()           //            
          .Offset(offset)   // int        
          .Limit(limit)     // int        
          .Query(query)     // List<string>
          .Select(columns)  // List<string>
          .Sort(sorts)      // string      
          .Delete()         //   
```
  
- Các thông tin về Init, Offset, Limit, Query, Select, Sort xem trong phần 6 (Get())

#### Trả về
Một số nguyên. Trong đó: 
  
- Các lỗi liên quan đến hàm Init, Offset, Limit, Query, Select, Sort xem trong phần 6 (Get()). Khi đó hệ thống trả về StatusCode.Error (không báo lỗi)
- Nếu gặp exception trong quá trình chuyển dữ liệu, hệ thống sẽ báo lỗi (có kèm thông tin exception), hủy bỏ quá trình xóa (thực hiện hàm Undo) và trả về StatusCode.Error
- Các trường hợp còn lại hệ thống sẽ báo thành công và trả về StatusCode.Success
  
---
  
### [UPDATE] 12. Cập nhật (các) phần tử thỏa mãn điều kiện - Update()
  
#### Cú pháp

```
<tên biến>.Init()             //            
          .Offset(offset)     // int        
          .Limit(limit)       // int        
          .Query(query)       // List<string>
          .Select(columns)    // List<string>
          .Sort(sorts)        // string      
          .Update(newValue)   // JObject  
```
  
- Các thông tin về Init, Offset, Limit, Query, Select, Sort xem trong phần 6 (Get())
- newValue: Giá trị mới cập nhật

#### Trả về
Một số nguyên. Trong đó: 
  
- Các lỗi liên quan đến hàm Init, Offset, Limit, Query, Select, Sort xem trong phần 6 (Get()). Khi đó hệ thống trả về StatusCode.Error (không báo lỗi)
- Nếu newValue là giá trị rỗng thì hệ thống báo lỗi và trả về StatusCode.Error
- Nếu tồn tại một cột PRIMARY KEY / NOT NULL mà trong element chứa giá trị null thì hệ thống báo lỗi và trả về StatusCode.Error
- Nếu tồn tại một cột PRIMARY KEY / UNIQUE mà trong element chứa giá trị trùng với các giá trị khác trong bảng thì hệ thống báo lỗi và trả về StatusCode.Error
- Nếu gặp exception trong quá trình chuyển dữ liệu, hệ thống sẽ báo lỗi (có kèm thông tin exception), hủy bỏ quá trình xóa (thực hiện hàm Undo) và trả về StatusCode.Error
- Các trường hợp còn lại hệ thống sẽ báo thành công và trả về StatusCode.Success
  
#### Lưu ý
  
- Để có được newValue là JObject có thể dùng hàm JObject.FromObject(...)
  
#### Ví dụ

```
Categories _newCategory = new()
{
    Id = (_maxIdRow == null) ? 0 : (_maxIdRow.Field<int>("Id") + 1),
    Name = _name,
    SubArray = new List<int>(),
    QuestionArray = new List<int>(),
    Description = _description,
    IdNumber = _id
};

if (categoriesData.Insert(JObject.FromObject(_newCategory)) == DataProcessing.StatusCode.Error)
    throw new Exception();
```
  
---
  
### 13. Chuyển từ DataRow sang JObject - ConvertDataRowToJObject()
  
#### Cú pháp
  
```
<tên biến>.ConvertDataRowToJObject(DataRow _dataRow)
```
  
#### Trả về
  
- JObject chuyển từ DataRow trên.
  
---
  
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
  
### [UPDATE] 15. Quay lại bản dữ liệu trước đó - Undo()
  
#### Cú pháp
  
```
<tên biến>.Undo()
```
  
#### Trả về
  
Không có

#### Lưu ý
  
- Thực chất trong class DataProcessing luôn có một biến PrevListElements lưu giá trị "backup" cho dữ liệu trước khi thực hiện các hàm Get(), GetFirstRow(), Insert(), DeleteAll(), Delete(), Update() tránh cho việc thực hiện hàm bị lỗi dữ liệu. Ví dụ: như khi thực hiện hàm Delete() với Update() bị lỗi thì sẽ tự động hủy bỏ hành động (bằng cách gọi làm Undo này).
  
- Biến PrevListElements chỉ lưu lại bản dữ liệu lần (gần nhất, ngay trước khi thực hiện hàm).
