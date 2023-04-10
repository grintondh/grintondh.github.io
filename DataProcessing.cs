using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Calculator
{
    public class DataProcessing
    {
        private static JArray ListElements { get; set; } = new JArray();
        private List<string> ShowColumnsName { get; set; } = new List<string>();
        private List<Type> ShowColumnsType { get; set; } = new List<Type>();
        public static int length { get; set; }
        private int Offset { get; set; }
        private int Limit { get; set; }

        private const int DEFAULT_LIMIT = 25;

        /// <summary>
        /// Import data file
        /// </summary>
        /// <param name="_jsonDataList">Data which is parsed in JArray.</param>
        /// IMPORTANT: Data needs to have "NotDelete" property
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
                {
                    Import(_columns, _columnsType);
                    return;
                }
                else
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
                    Limit = DEFAULT_LIMIT;
                }
                else
                    throw new Exception();

            }
            catch (Exception ex)
            {
                DialogResult _errorDialog = MessageBox.Show("Couldn't import data\n" + ex, "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);

                if (_errorDialog == DialogResult.Retry)
                {
                    Import(_jsonDataList);
                    return;
                }
                else
                    return;
            }
        }

        /// <summary>
        /// Export data to DataGridView.DataSource object
        /// </summary>
        /// <param name="_offset">First index you want to get</param>
        /// <param name="_limit">Number of elements you want to get</param>
        /// <return>Return DataTable</return>
        public DataTable GetList(int _offset, int _limit)
        {
            try
            {
                Offset = _offset;
                Limit = _limit;

                /// Correction offset and limit value
                Limit = Math.Min(Math.Max(0, Limit), length);
                Offset = Math.Min(Math.Max(0, Offset), length - Limit);

                DataTable _dataTable = new DataTable();

                for (int _j = 0; _j < ShowColumnsName.Count; _j++)
                {
                    string _columnName = ShowColumnsName[_j];
                    Type _columnType = ShowColumnsType[_j];
                    _dataTable.Columns.Add(_columnName, _columnType);
                }

                for (int _i = Offset; _i < Offset + Limit; _i++)
                {
                    DataRow _dataRow = _dataTable.NewRow();

                    for (int _j = 0; _j < ShowColumnsName.Count; _j++)
                    {
                        string _columnName = ShowColumnsName[_j];
                        _dataRow[_columnName] = ListElements[_i][_columnName];
                    }

                    _dataTable.Rows.Add(_dataRow);
                }

                return _dataTable;
            }
            catch (Exception ex)
            {
                DialogResult _errorDialog = MessageBox.Show("Couldn't get or show data\n" + ex, "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);

                if (_errorDialog == DialogResult.Retry)
                {
                    GetList(_offset, _limit);
                    return null;
                }
                else
                    return null;
            }
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
        public Tuple<int,int> GetOffsetLimitNow()
        {
            Tuple<int, int> _tuple = Tuple.Create<int, int>(Offset, Limit);
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

                    DataTable _x = GetList(length + 1, Limit);
                    MessageBox.Show("Added new element!", "Success", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

                    return _x;
                }
                else
                    throw new Exception();
            }
            catch (Exception ex)
            {
                DialogResult _errorDialog = MessageBox.Show("Couldn't add a new element\n" + ex, "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);

                if (_errorDialog == DialogResult.Retry)
                {
                    AddNewElement(_element);
                    return null;
                }
                else
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

            return GetList(length + 1, Limit);
        }

        /// <summary>
        /// Update elements (and delete selected rows, using Delete checkbox column) in [Offset, Offset + Limit) range
        /// </summary>
        /// <param name="_dataTable">(DataTable) DataGridView.DataSource</param>
        /// <return>Return DataTable</return>
        public DataTable UpdateElementsInRange(DataTable _dataTable)
        {
            if (_dataTable != null)
            {
                for (int _i = Offset + Limit - 1; _i >= Offset; _i--)
                    if (_dataTable.Rows[_i - Offset].Field<bool>("delete") == true && _dataTable.Rows[_i - Offset].Field<bool>("NotDelete") == false)
                    {
                        ListElements.RemoveAt(_i);
                        length--;
                    }
                    else if (_dataTable.Rows[_i - Offset].Field<bool>("delete") == false)
                    {
                        for (int _j = 0; _j < ShowColumnsName.Count; _j++)
                        {
                            string _columnName = ShowColumnsName[_j];
                            ListElements[_i][_columnName] = JToken.FromObject(_dataTable.Rows[_i - Offset].ItemArray[_j]);
                        }
                    }

                DataTable _x = GetList(Offset, Limit);
                MessageBox.Show("Updated all elements!", "Success", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                return _x;
            }
            else
            {
                MessageBox.Show("Couldn't get / update / delete element(s)\nYour DataGridView.DataSource is empty", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                return null;
            }
        }

        /// <summary>
        /// Delete a element in [Offset, Offset + Limit) range
        /// </summary>
        /// <param name="_dataTable">(DataTable) DataGridView.DataSource</param>
        /// <param name="_indexInTable">Index in Table (with Offset and Limit)</param>
        /// <returns>new DataTable, if it gets error, return null</returns>
        public DataTable DeleteElementInRange(DataTable _dataTable, int _indexInTable)
        {
            if (_dataTable == null || _indexInTable >= Limit)
            {
                MessageBox.Show("Couldn't delete element", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                return null;
            }
            else
            {
                if (_dataTable.Rows[_indexInTable + Offset].Field<bool>("NotDelete") == false)
                {
                    ListElements.RemoveAt(_indexInTable + Offset);
                    length--;

                    DataTable _x = GetList(Offset, Limit);
                    MessageBox.Show("Deleted element!", "Success", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    return _x;
                }
                else
                {
                    MessageBox.Show("You don't have permission to delete this element", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    return null;
                }
            }
        }
        /// <summary>
        /// Change Element in [Offset, Offset + Limit) range
        /// </summary>
        /// <param name="_dataTable">(DataTable) DataGridView.DataSource</param>
        /// <param name="_indexInTable">Index in Table (with Offset and Limit)</param>
        /// <returns></returns>
        public DataTable ChangeElementInRange(DataTable _dataTable, int _indexInTable)
        {
            if (_dataTable == null || _indexInTable >= Limit)
            {
                MessageBox.Show("Couldn't change element", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                return null;
            }
            else
            {
                for (int _j = 0; _j < ShowColumnsName.Count; _j++)
                {
                    string _columnName = ShowColumnsName[_j];
                    ListElements[_indexInTable + Offset][_columnName] = JToken.FromObject(_dataTable.Rows[_indexInTable + Offset].ItemArray[_j]);
                }

                DataTable _x = GetList(Offset, Limit);
                MessageBox.Show("Changed element!", "Success", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                return _x;
            }
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
    }
}
