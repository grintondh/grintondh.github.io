// DataProcessing.cs v3.0.0
// Last modified: 26.4.2023 by DH

using Newtonsoft.Json.Linq;
using System.Data;

namespace Learning_System.ExternalClass
{
    public class DataProcessing
    {
        private JArray ListElements { get; set; } = new JArray();
        
        /// <summary>
        /// Lưu ListElements ngay trước đó. Cập nhật trước khi thực hiện hàm Get(), GetFirstRow(), Insert(), Update(), DeleteAll(), Delete()
        /// </summary>
        private JArray PrevListElements { get; set; } = new JArray();

        private List<(string Name, Type Type, string Key)> ColumnsSetting { get; set; } = new List<(string Name, Type Type, string Key)>();
        public static List<string> EmptyList { get; } = new List<string>();

        private const int DEFAULT_LIMIT = 25;

        public class StatusCode
        {
            public const int OK = 1;
            public const int Error = 2;
            public const int Unset = 0;
        }

        /// <summary>
        /// Nhập dữ liệu (cột)
        /// </summary>
        /// <param name="_columns">Tên các cột</param>
        /// <param name="_columnsType">Loại dữ liệu của các cột</param>
        /// <param name="_columnsKey">Khóa hằng số: PRIMARY KEY, NOT NULL, UNIQUE</param>
        /// <returns>StatusCode: OK (1): Thành công; Error (2): Thất bại</returns>
        public int Import(List<string> _columns, List<Type> _columnsType, List<string> _columnsKey)
        {
            if (_columnsKey.Contains("PRIMARY KEY") == false)
            {
                MessageBox.Show("Cần có ít nhất một cột có tính chất Primary (Unique + Not null) trong bảng!\n", "Error");
                return StatusCode.Error;
            }
            
            if (_columns.Count != _columnsType.Count || _columns.Count != _columnsKey.Count)
            {
                MessageBox.Show("Số lượng các trường danh sách của cột không tương thích với nhau!\n", "Error");
                return StatusCode.Error;
            }

            for (int _i = 0; _i < _columns.Count; _i++)
            {
                (string Name, Type Type, string Key) _newRow = (_columns[_i], _columnsType[_i], _columnsKey[_i]);
                ColumnsSetting.Add(_newRow);
            }

            return StatusCode.OK;
        }

        /// <summary>
        /// Nhập dữ liệu (hàng)
        /// </summary>
        /// <param name="_jsonDataList">Danh sách dữ liệu đã xuất ra từ file JSON</param>
        /// <returns>StatusCode: OK (1): Thành công; Error (2): Thất bại</returns>
        public int Import(JArray _jsonDataList)
        {
            try
            {
                if (_jsonDataList != null)
                {
                    ListElements.Clear();

                    foreach (JObject _jsonObj in _jsonDataList.Cast<JObject>())
                        ListElements.Add(_jsonObj);

                    return StatusCode.OK;
                }
                else
                {
                    MessageBox.Show("Danh sách đầu vào rỗng!\n", "Error");
                    return StatusCode.Error;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi trong quá trình chuyển đổi dữ liệu!\nChi tiết lỗi:\n" + ex, "Error");
                return StatusCode.Error;
            }
        }

        private List<string> queryList = new();
        private List<string> columnsList = new();
        private string? sortList = null;
        private int limit = DEFAULT_LIMIT;
        private int offset = 0;

        /// <summary>
        /// Lấy danh sách thỏa mãn điều kiện. Đây là hàm private.
        /// </summary>
        /// <returns>null nếu thất bại, List các index thỏa mãn nếu thành công.</returns>
        private List<int>? GetAllSatisfy()
        {
            try
            {
                if (queryList == null || columnsList == null)
                {
                    MessageBox.Show("Danh sách truy vấn và danh sách cột không thể là null (sử dụng .EmptyList cho list rỗng)!", "Error");
                    return null;
                }

                if (queryList.Count % 2 != 0)
                {
                    MessageBox.Show("Danh sách truy vấn cần có số chẵn các phần tử!", "Error");
                    return null;
                }

                bool _existPrimaryKeyOrUnique = false;

                foreach (var _columnObj in ColumnsSetting)
                    if (_columnObj.Key.Contains("PRIMARY KEY") == true || _columnObj.Key.Contains("UNIQUE") == true)
                    {
                        _existPrimaryKeyOrUnique = true;
                        break;
                    }

                if (_existPrimaryKeyOrUnique == false)
                    MessageBox.Show("Nên có ít nhất một cột PRIMARY KEY hoặc UNIQUE trong danh sách cột!", "Warning!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                List<int> _choseIndex = new();
                int length = ListElements.Count;

                /// Correct offset and limit values
                limit = Math.Min(Math.Max(0, limit), length);
                offset = Math.Min(Math.Max(0, offset), length - limit);

                int _gotCount = 0;

                for (int _i = offset; _i < length && _gotCount < limit; _i++)
                {
                    bool _isSatisfy = true;

                    if (queryList.Count > 0)
                    {
                        for (int _j = 0; _j < queryList.Count; _j += 2)
                        {
                            var _elementObj = ListElements[_i][queryList[_j]];
                            if (_elementObj == null)
                                continue;

                            string x = _elementObj.ToString();

                            if (queryList[_j + 1].Length > 7 && queryList[_j + 1][..7] == "CONTAIN")
                            {
                                string _compareValue = queryList[_j + 1][8..];

                                if (x.Contains(_compareValue) == false)
                                {
                                    _isSatisfy = false;
                                    break;
                                }
                            }
                            else if (x != queryList[_j + 1])
                            {
                                _isSatisfy = false;
                                break;
                            }
                        }
                    }

                    if (_isSatisfy == false)
                        continue;

                    _choseIndex.Add(_i);
                    _gotCount++;
                }

                return _choseIndex;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể duyệt dữ liệu!\nChi tiết lỗi:\n" + ex, "Error");
                return null;
            }
        }

        /// <summary>
        /// Lấy dữ liệu (kết hợp với Offset(), Limit(), Query(), Select(), Sort());
        /// </summary>
        /// <returns>null nếu thất bại, DataTable nếu thành công</returns>
        public DataTable? Get()
        {
            PrevListElements = ListElements;

            try
            {
                List<int>? getIndex = GetAllSatisfy();

                if (getIndex == null)    
                    return null;

                DataTable _dataTable = new();

                /// Add DataTable columns
                if (columnsList.Count > 0)
                {
                    for (int _j = 0; _j < columnsList.Count; _j++)
                    {
                        string _columnName = columnsList[_j];
                        bool _isExistColumn = false;

                        for (int _k = 0; _k < ColumnsSetting.Count; _k++)
                        {
                            if (_columnName == ColumnsSetting[_k].Name)
                            {
                                _dataTable.Columns.Add(ColumnsSetting[_k].Name, ColumnsSetting[_k].Type);
                                _isExistColumn = true;
                                break;
                            }
                        }

                        if (_isExistColumn == false)
                        {
                            MessageBox.Show("Không tồn tại cột " + _columnName + "!", "Error");
                            return null;
                        }
                    }
                }
                else
                {
                    for (int _j = 0; _j < ColumnsSetting.Count; _j++)
                        _dataTable.Columns.Add(ColumnsSetting[_j].Name, ColumnsSetting[_j].Type);
                }

                /// Add DataTable rows
                foreach(int _index in getIndex)
                {
                    DataRow _dataRow = _dataTable.NewRow();

                    if (columnsList.Count > 0)
                    {
                        for (int _j = 0; _j < columnsList.Count; _j++)
                        {
                            string _columnName = columnsList[_j];
                            _dataRow[_columnName] = ListElements[_index][_columnName];
                        }
                    }
                    else
                    {
                        for (int _j = 0; _j < ColumnsSetting.Count; _j++)
                        {
                            string _columnName = ColumnsSetting[_j].Name;
                            _dataRow[_columnName] = ListElements[_index][_columnName];
                        }
                    }

                    _dataTable.Rows.Add(_dataRow);
                }

                if (sortList != null)
                {
                    DataView _dv = _dataTable.DefaultView;
                    _dv.Sort = sortList;
                    _dataTable = _dv.ToTable();
                }

                return _dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể lấy dữ liệu!\nChi tiết lỗi:\n" + ex, "Error");
                return null;
            }
        }

        /// <summary>
        /// Reset các điều kiện
        /// </summary>
        public DataProcessing Init()
        {
            queryList = EmptyList;
            columnsList = EmptyList;
            sortList = null;
            limit = DEFAULT_LIMIT;
            offset = 0;
            return this;
        }

        /// <summary>
        /// Chỉ số của bản ghi xét đầu tiên
        /// </summary>
        public DataProcessing Offset(int _offset)
        {
            offset = _offset;
            return this;
        }

        /// <summary>
        /// Số lượng tối đa bản ghi trả về
        /// </summary>
        public DataProcessing Limit(int _limit)
        {
            limit = _limit;
            return this;
        }

        /// <summary>
        /// Thêm truy vấn cho bản ghi
        /// </summary>
        /// <param name="_queryList">Danh sách truy vấn gồm số chẵn các phần tử. Truy vấn thứ i có phần tử 2 * i là tên cột, 2 * i + 1 là giá trị hoặc giá trị con (CONTAIN)"</param>
        /// <returns></returns>
        public DataProcessing Query(List<string> _queryList)
        {
            queryList = _queryList;
            return this;
        }

        /// <summary>
        /// Danh sách các cột cần lấy (nên chứa ít nhất một cột PRIMARY KEY hoặc UNIQUE)
        /// </summary>
        public DataProcessing Select(List<string> _columnsList)
        {
            columnsList = _columnsList;
            return this;
        }

        /// <summary>
        /// Thứ tự sắp xếp
        /// </summary>
        /// <param name="_sortList">Điều kiện sắp xếp, có dạng “... asc/desc", cách nhau bởi dấu phẩy.</param>
        /// <returns></returns>
        public DataProcessing Sort(string _sortList)
        {
            sortList = _sortList;
            return this;
        }

        /// <summary>
        /// Lấy dữ liệu hàng đầu tiên trả về (kết hợp với Offset(), Limit(), Query(), Select(), Sort())
        /// </summary>
        /// <returns>null nếu thất bại, DataRow nếu thành công</returns>
        public DataRow? GetFirstRow()
        {
            DataTable? _dt = Get();
            
            if (_dt == null)
                return null;
            else
                return _dt.Rows[0];
        }

        /// <summary>
        /// Số lượng bản ghi
        /// </summary>
        public int Length()
        {
            return ListElements.Count;
        }

        /// <summary>
        /// Thêm bản ghi mới
        /// </summary>
        /// <param name="_element">Bản ghi mới. Thỏa mãn các yêu cầu về khóa hằng số.</param>
        /// <returns>StatusCode: OK (1): Thành công; Error (2): Thất bại</returns>
        public int Insert(JObject _element)
        {
            PrevListElements = ListElements;

            if(_element == null)
            {
                MessageBox.Show("Không thể thêm phần tử mới: Phần tử mới rỗng (null)!", "Error");
                return StatusCode.Error;
            }

            try
            {
                foreach (var _col in ColumnsSetting)
                {
                    if ((_col.Key.Contains("NOT NULL") == true || _col.Key.Contains("PRIMARY KEY") == true) && _element[_col.Name] == null)
                    {
                        MessageBox.Show("Không thể thêm phần tử mới: " + _col.Name + " chứa giá trị NULL trong khi cột được đặt là NOT NULL!", "Error");
                        return StatusCode.Error;
                    }
                    else if (_col.Key.Contains("PRIMARY KEY") == true || _col.Key.Contains("UNIQUE") == true)
                    {
                        foreach (var _row in ListElements)
                            if (_element[_col.Name].ToString().ToLower() == _row[_col.Name].ToString().ToLower())
                            {
                                MessageBox.Show("Không thể thêm phần tử mới: " + _col.Name + " chứa giá trị trùng lặp trong khi cột được đặt là UNIQUE!", "Error");
                                return StatusCode.Error;
                            }
                    }
                }

                ListElements.Add(_element);
                MessageBox.Show("Đã thêm phần tử mới!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return StatusCode.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể thêm phần tử mới: Có lỗi trong quá trình duyệt dữ liệu\nChi tiết lỗi:\n" + ex, "Error");
                return StatusCode.Error;
            }
        }

        /// <summary>
        /// Xóa tất cả bản ghi
        /// </summary>
        public void DeleteAll()
        {
            PrevListElements = ListElements;
            ListElements.Clear();
        }

        /// <summary>
        /// Xóa (các) bản ghi thỏa mãn điều kiện (kết hợp với Offset(), Limit(), Query(), Select(), Sort())
        /// </summary>
        /// <returns>StatusCode: OK (1): Thành công; Error (2): Thất bại</returns>
        public int Delete()
        {
            PrevListElements = ListElements;

            if(this == null)
                return StatusCode.Error;

            try
            {
                List<int>? removeList = GetAllSatisfy();

                if (removeList == null)
                    return StatusCode.Error;

                removeList.Reverse();
                foreach (var item in removeList)
                    ListElements.RemoveAt(item);

                MessageBox.Show("Đã xóa thành công (các) phần tử!", "Success");
                return StatusCode.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể xóa phần tử: Có lỗi trong quá trình xóa. Hệ thống đã tự động quay về bản dữ liệu trước khi thực hiện thao tác.\nChi tiết lỗi:\n" + ex, "Error");
                Undo();
                return StatusCode.Error;
            }
        }

        /// <summary>
        /// Cập nhật (các) bản ghi thỏa mãn điều kiện (kết hợp với Offset(), Limit(), Query(), Select(), Sort())
        /// </summary>
        /// <returns>StatusCode: OK (1): Thành công; Error (2): Thất bại</returns>
        public int Update(JObject _newValue)
        {
            PrevListElements = ListElements;

            if(this == null)
                return StatusCode.Error;

            if(_newValue == null)
            {
                MessageBox.Show("Không thể cập nhật phần tử: Phần tử cập nhật rỗng!", "Error");
                return StatusCode.Error;
            }
            try
            {
                List<int>? updateList = GetAllSatisfy();

                if(updateList == null)
                    return StatusCode.Error;

                foreach (var _col in ColumnsSetting)
                {
                    if ((_col.Key.Contains("NOT NULL") == true || _col.Key.Contains("PRIMARY KEY") == true) && _newValue[_col.Name] == null)
                    {
                        MessageBox.Show("Không thể cập nhật phần tử: " + _col.Name + " chứa giá trị NULL trong khi cột được đặt là NOT NULL!", "Error");
                        return StatusCode.Error;
                    }
                    else if (_col.Key.Contains("PRIMARY KEY") == true || _col.Key.Contains("UNIQUE") == true)
                    {
                        for (int _i = 0; _i < ListElements.Count; _i++)
                            if (updateList.Contains(_i) == false && _newValue[_col.Name].ToString().ToLower() == ListElements[_i][_col.Name].ToString().ToLower())
                            {
                                MessageBox.Show("Không thể cập nhật phần tử: " + _col.Name + " chứa giá trị trùng lặp trong khi cột được đặt là UNIQUE!", "Error");
                                return StatusCode.Error;
                            }                                
                    }
                }

                foreach (var item in updateList)
                    foreach (JProperty jProperty in _newValue.Properties())
                        ListElements[item][jProperty.Name] = jProperty.Value;

                MessageBox.Show("Đã cập nhật thành công (các) phần tử!", "Success");
                return StatusCode.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể cập nhật phần tử: Có lỗi trong quá trình cập nhật. Hệ thống đã tự động quay về bản dữ liệu trước khi thực hiện thao tác.\nChi tiết lỗi:\n" + ex, "Error");
                Undo();
                return StatusCode.Error;
            }
        }

        public static JObject ConvertDataRowToJObject(DataRow _dataRow)
        {
            JObject _returnObj = new();

            foreach (DataColumn _col in _dataRow.Table.Columns)
                _returnObj.Add(_col.ColumnName, JToken.FromObject(_dataRow[_col]));

            return _returnObj;
        }

        public JArray Export()
        {
            JArray _returnToken = new();
            foreach (var _p in ListElements)
                _returnToken.Add(_p);

            JArray _returnData = JArray.FromObject(_returnToken);

            return _returnData;
        }

        /// <summary>
        /// Quay lại bản dữ liệu trước đó
        /// </summary>
        public void Undo()
        {
             ListElements = PrevListElements;
        }
    }
}
