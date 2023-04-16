using Accessibility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using System.Windows.Forms;

namespace Learning_System.ExternalClass
{
    public class DataProcessing
    {
        private JArray ListElements { get; set; } = new JArray();
        private List<string> ShowColumnsName { get; set; } = new List<string>();
        private List<Type> ShowColumnsType { get; set; } = new List<Type>();
        private int length { get; set; }
        private int Limit { get; set; }
        private List<string> Condition { get; set; } = new List<string>();
        private List<string> Columns { get; set; } = new List<string>();
        private int Offset { get; set; }
        private List<Tuple<int, int>> SelectedRow { get; set; } = new List<Tuple<int, int>>();
        public static List<string> emptyList { get; } = new List<string>();

        private const int DEFAULT_LIMIT = 25;
        
        /// <summary>
        /// Import data file
        /// </summary>
        /// <param name="_jsonDataList">Data which is parsed in JArray.</param>
        /// <param name="_columns">List of columns' name you want to show</param>
        /// <param name="_columnsType">List of columns' type you want to show</param>
        public void Import(List<string> _columns, List<Type> _columnsType)
        {
            try
            {
                ShowColumnsName = _columns;
                if (ShowColumnsName.Contains("delete") == false)
                    ShowColumnsName.Add("delete");

                ShowColumnsType = _columnsType;
                ShowColumnsType.Add(typeof(bool));
            }
            catch (Exception ex)
            {
                DialogResult _errorDialog = MessageBox.Show("Couldn't import data\n" + ex, "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);

                if (_errorDialog == DialogResult.Retry)
                    Import(_columns, _columnsType);
                    
                return;
            }
        }
        public void Import(JArray _jsonDataList)
        {
            try
            {
                if (_jsonDataList != null)
                {
                    ListElements.Clear();

                    bool _defaultBoolValue = false;
                    foreach (JObject _jsonObj in _jsonDataList)
                    {
                        if (_jsonObj["delete"] != null)
                            _jsonObj["delete"] = _defaultBoolValue;
                        else
                            _jsonObj.Add(new JProperty("delete", _defaultBoolValue));

                        ListElements.Add(_jsonObj);
                    }

                    length = ListElements.Count;
                }
                else
                    throw new Exception();

            }
            catch (Exception ex)
            {
                DialogResult _errorDialog = MessageBox.Show("Couldn't import data\n" + ex, "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);

                if (_errorDialog == DialogResult.Retry)
                    Import(_jsonDataList);
                    
                return;
            }
        }

        /// <summary>
        /// Export data to DataGridView.DataSource object
        /// </summary>
        /// <param name="_offset">First index you want to get</param>
        /// <param name="_limit">Number of elements you want to get</param>
        /// <param name="_query">Queries List (optional)</param>
        /// <param name="_columns">Columns List (optional)</param>
        /// <return>Return DataTable</return>
        public DataTable GetList(int _offset, int _limit, List<string> _query, List<string> _columns, string _sort)
        {
            try
            {
                if (_query.Count % 2 != 0 && (_query.Count != 1 || _query[0] != "SAME"))
                    throw new Exception();

                int _archievedOffset = Offset, _archievedLimit = Limit;

                Offset = _offset;
                Limit = _limit;

                if (_query.Count == 1 && _query[0] == "SAME")
                    Condition = Condition;
                else
                    Condition = _query;

                if (_columns.Count == 1 && _columns[0] == "SAME")
                    Columns = Columns;
                else
                    Columns = _columns;

                /// Correction offset and limit value
                Limit = Math.Min(Math.Max(0, Limit), length);
                Offset = Math.Min(Math.Max(0, Offset), length - Limit);

                DataTable _dataTable = new DataTable();

                if (Columns.Count > 0)
                {
                    for (int _j = 0; _j < Columns.Count; _j++)
                    {
                        string _columnName = Columns[_j];
                        bool _isExistColumn = false;

                        for (int _k = 0; _k < ShowColumnsName.Count; _k++)
                        {
                            if (_columnName == ShowColumnsName[_k])
                            {
                                Type _columnType = ShowColumnsType[_k];
                                _dataTable.Columns.Add(_columnName, _columnType);

                                _isExistColumn = true;
                                break;
                            }
                        }

                        if (_isExistColumn == false)
                            throw new Exception();
                    }
                }
                else
                {
                    for (int _j = 0; _j < ShowColumnsName.Count; _j++)
                    {
                        string _columnName = ShowColumnsName[_j];
                        Type _columnType = ShowColumnsType[_j];
                        _dataTable.Columns.Add(_columnName, _columnType);
                    }
                }

                SelectedRow.Clear();

                int _gotCount = 0;

                for (int _i = Offset; _i < length && _gotCount < Limit; _i++)
                {
                    DataRow _dataRow = _dataTable.NewRow();

                    bool _isSatisfy = true;

                    if (Condition.Count > 0)
                    {
                        for (int _j = 0; _j < Condition.Count; _j += 2)
                        {
                            if (Condition[_j] == "GetMaxOn")
                                continue;

                            var _elementObj = ListElements[_i][Condition[_j]];
                            if (_elementObj == null)
                                continue;

                            string x = _elementObj.ToString();

                            if (Condition[_j + 1].Length > 7 && Condition[_j + 1].Substring(0, 7) == "CONTAIN")
                            {
                                string _compareValue = Condition[_j + 1].Substring(8, Condition[_j + 1].Length - 8);

                                if (x.Contains(_compareValue) == false)
                                {
                                    _isSatisfy = false;
                                    break;
                                }
                            }
                            else if (x != Condition[_j + 1])
                            {
                                _isSatisfy = false;
                                break;
                            }
                        }
                    }

                    if (_isSatisfy == false)
                        continue;

                    SelectedRow.Add(Tuple.Create(_i, _gotCount));

                    _gotCount++;

                    if (Columns.Count > 0)
                    {
                        for (int _j = 0; _j < Columns.Count; _j++)
                        {
                            string _columnName = Columns[_j];
                            _dataRow[_columnName] = ListElements[_i][_columnName];
                        }
                    }
                    else
                    {
                        for (int _j = 0; _j < ShowColumnsName.Count; _j++)
                        {
                            string _columnName = ShowColumnsName[_j];
                            _dataRow[_columnName] = ListElements[_i][_columnName];
                        }
                    }

                    _dataTable.Rows.Add(_dataRow);
                }

                Limit = _gotCount;

                if(_sort != null)
                {
                    DataView _dv = _dataTable.DefaultView;
                    _dv.Sort = _sort;
                    _dataTable = _dv.ToTable();
                }

                return _dataTable;
            }
            catch (Exception ex)
            {
                DialogResult _errorDialog = MessageBox.Show("Couldn't get or show data\n" + ex, "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);

                if (_errorDialog == DialogResult.OK)
                    Application.Exit();
                return null;
            }
        }

        public DataTable GetList(int _offset, int _limit)
        {
            return GetList(_offset, _limit, emptyList, emptyList, null);
        }

        public DataTable GetList(int _offset, int _limit, string _sort)
        {
            return GetList(_offset, _limit, emptyList, emptyList, _sort);
        }

        public DataTable GetList(int _offset, int _limit, List<string> _query, string _sort)
        {
            return GetList(_offset, _limit, _query, emptyList, _sort);
        }

        public DataTable GetList(int _offset, int _limit, List<string> _query, List<string> _columns)
        {
            return GetList(_offset, _limit, _query, _columns, null);
        }

        /// <summary>
        /// Return max / min datarow satisfy conditions
        /// </summary>
        /// <param name="_offset">Offset</param>
        /// <param name="_limit">Limit</param>
        /// <param name="_query">Conditions list</param>
        /// <param name="_sort">Sort conditions</param>
        /// <param name="_maxMin">MAX / MIN</param>
        /// <returns>A datarow</returns>
        public DataRow GetMaxMin(int _offset, int _limit, List<string> _query, string _sort, string _maxMin)
        {
            DataTable _sortedDB = GetList(_offset, _limit, _query, _sort);

            if (_maxMin == "MAX")
                return _sortedDB.Rows[_sortedDB.Rows.Count - 1];
            else if (_maxMin == "MIN")
                return _sortedDB.Rows[0];
            else
                return null;
        }

        /// <summary>
        /// Return number of elements
        /// </summary>
        /// <returns></returns>
        public int GetLength()
        {
            return length;
        }

        /// <summary>
        /// Return Offset and Limit value at the moment
        /// </summary>
        /// <returns></returns>
        public Tuple<int, int> GetOffsetLimitNow()
        {
            Tuple<int, int> _tuple = Tuple.Create(Offset, Limit);
            return _tuple;
        }

        /// <summary>
        /// Add a new element to data and show new data grid view
        /// </summary>
        /// <param name="_element">Element in JObject type</param>
        /// <return>Return DataTable</return>
        public DataTable AddNewElement(JObject _element)
        {
            try
            {
                if (_element != null)
                {
                    _element.Add(new JProperty("delete", false));
                    ListElements.Add(_element);

                    length++;

                    DataTable _x = GetList(length + 1, DEFAULT_LIMIT, Condition, Columns);
                    MessageBox.Show("Added new element!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    return _x;
                }
                else
                    throw new Exception();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Couldn't add a new element\n" + ex, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        /// <summary>
        /// Delete all elements in data source
        /// </summary>
        /// <return>Return DataTable</return>
        public DataTable DeleteAllElements()
        {
            ListElements.Clear();
            length = 0;

            return GetList(length + 1, Limit, Condition, Columns);
        }

        /// <summary>
        /// Delete a element in range
        /// </summary>
        /// <param name="_dataTable">(DataTable) DataGridView.DataSource</param>
        /// <param name="_indexInTable">Index in Table (with Offset and Limit)</param>
        public void DeleteElementInRange(DataTable _dataTable, int _indexInTable)
        {
            if (_dataTable == null || _indexInTable >= _dataTable.Rows.Count)
            {
                MessageBox.Show("Couldn't delete element", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                if (_dataTable.Rows[_indexInTable].Table.Columns.Contains("NotDelete") == true)
                {
                    if (_dataTable.Rows[_indexInTable].Field<bool>("NotDelete") == false)
                    {
                        foreach (var _i in SelectedRow)
                            if (_i.Item2 == _indexInTable)
                            {
                                ListElements.RemoveAt(_i.Item1);
                                length--;

                                break;
                            }

                        return;
                    }
                    else
                    {
                        MessageBox.Show("You don't have permission to delete this element", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    foreach (var _i in SelectedRow)
                        if (_i.Item2 == _indexInTable)
                        {
                            ListElements.RemoveAt(_i.Item1);
                            length--;

                            break;
                        }

                    return;
                }
            }
        }
        /// <summary>
        /// Change Element in range
        /// </summary>
        /// <param name="_dataTable">(DataTable) DataGridView.DataSource</param>
        /// <param name="_indexInTable">Index in Table (with Offset and Limit)</param>
        public void ChangeElementInRange(DataTable _dataTable, int _indexInTable)
        {
            if (_dataTable == null || _indexInTable >= _dataTable.Rows.Count)
            {
                MessageBox.Show("Couldn't change element", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                foreach (var _i in SelectedRow)
                    if (_i.Item2 == _indexInTable)
                    {
                        if (Columns != null)
                        {
                            for (int _j = 0; _j < Columns.Count; _j++)
                            {
                                string _columnName = Columns[_j];
                                ListElements[_i.Item1][_columnName] = JToken.FromObject(_dataTable.Rows[_i.Item2].ItemArray[_j]);
                            }
                        }
                        else
                        {
                            for (int _j = 0; _j < ShowColumnsName.Count; _j++)
                            {
                                string _columnName = ShowColumnsName[_j];
                                ListElements[_i.Item1][_columnName] = JToken.FromObject(_dataTable.Rows[_i.Item2].ItemArray[_j]);
                            }
                        }

                        break;
                    }

                return;
            }
        }

        /// <summary>
        /// Change all Elements which satisfy with conditions
        /// </summary>
        /// <param name="_query">Queries List</param>
        /// <param name="_newValue">New value in JObject</param>
        /// <return>Return DataTable</return>
        public void ChangeElementswithCondition(List<string> _query, JObject _newValue)
        {
            try
            {
                if (_query.Count % 2 != 0 && (_query.Count != 1 || _query[0] != "SAME"))
                    throw new Exception();

                DataTable _dataTable = new DataTable();

                for (int _i = 0; _i < length; _i++)
                {
                    bool _isSatisfy = true;

                    if (Condition.Count > 0)
                    {
                        for (int _j = 0; _j < Condition.Count; _j += 2)
                        { 
                            var _elementObj = ListElements[_i][Condition[_j]];
                            if (_elementObj == null)
                                continue;

                            string x = _elementObj.ToString();

                            if (Condition[_j + 1].Length > 7 && Condition[_j + 1].Substring(0, 7) == "CONTAIN")
                            {
                                string _compareValue = Condition[_j + 1].Substring(7, Condition[_j + 1].Length - 7);

                                if (x.Contains(_compareValue) == false)
                                {
                                    _isSatisfy = false;
                                    break;
                                }
                            }
                            else if (x != Condition[_j + 1])
                            {
                                _isSatisfy = false;
                                break;
                            }
                        }
                    }

                    if (_isSatisfy == false)
                        continue;

                    foreach(JProperty jProperty in _newValue.Properties())
                        ListElements[_i][jProperty.Name] = jProperty.Value;

                }

                return;
            }
            catch (Exception ex)
            {
                DialogResult _errorDialog = MessageBox.Show("Couldn't get or change data\n" + ex, "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);

                if (_errorDialog == DialogResult.OK)
                    Application.Exit();
                return;
            }
        }
        public static JObject ConvertDataRowToJObject(DataRow _dataRow)
        {
            JObject _returnObj = new JObject();

            foreach(DataColumn _col in _dataRow.Table.Columns)
                _returnObj.Add(_col.ColumnName, JToken.FromObject(_dataRow[_col]));

            return _returnObj;
        }
        public JArray Export()
        {
            JArray _returnToken = new JArray();
            foreach (var _p in ListElements)
                _returnToken.Add(_p);

            JArray _returnData = JArray.FromObject(_returnToken);

            _returnData.Descendants()
            .OfType<JProperty>()
            .Where(x => x.Name == "delete")
            .ToList()
            .ForEach(x => x.Remove());

            MessageBox.Show("Export all data!", "Success", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

            return _returnData;
        }

        /// <summary>
        /// Copy data to another DataProcessing _x
        /// </summary>
        /// <param name="_x"></param>
        public void CopyData(DataProcessing _x)
        {
            List<string> _newColumnName = new List<string>(ShowColumnsName);
            List<Type> _newColumnType = new List<Type>(ShowColumnsType);
            JArray _newListElements = new JArray(ListElements);

            _x.Import(_newColumnName, _newColumnType);
            _x.Import(_newListElements);
        }
    }
}
