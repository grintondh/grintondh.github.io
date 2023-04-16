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
### Vào [đây](DataProcessing) để xem về file này.
