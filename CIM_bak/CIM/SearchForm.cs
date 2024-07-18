using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CIM
{
    public partial class SearchForm : UIForm
    {
        public SearchForm()
        {
            InitializeComponent();
        }

        private void SearchForm_Load(object sender, EventArgs e)
        {

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            string Qrcode = txtQR.Text;
            DataSet ds = SqlLite.Instance.SearchData(Qrcode);
            DataTable dt = ds.Tables[0];
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    
                   int rowIndex = dataGridView1.Rows.Add(
                        row["TOPHOUSING"],
                        row["BOX1_GLUE_AMOUNT"],
                        row["BOX1_GLUE_DISCHARGE_VOLUME_VISION"],
                        row["INSULATOR_BAR_CODE"],
                        row["BOX1_GLUE_OVERFLOW_VISION"],
                        row["BOX1_HEATED_AIR_CURING"],
                        row["BOX2_GLUE_AMOUNT"],
                        row["BOX2_GLUE_DISCHARGE_VOLUME_VISION"],
                        row["FPCB_BAR_CODE"],
                        row["BOX2_GLUE_OVERFLOW_VISION"],
                        row["BOX2_HEATED_AIR_CURING"],
                        row["BOX3_DISTANCE"],
                        row["BOX3_GLUE_AMOUNT"],
                        row["BOX3_GLUE_DISCHARGE_VOLUME_VISION"],
                        row["BOX3_GLUE_OVERFLOW_VISION"],
                        row["BOX3_HEATED_AIR_CURING"],
                        row["BOX4_TIGHTNESS_AND_LOCATION_VISION"],
                        row["BOX4_HEIGHT_PARALLELISM"],
                        row["BOX4_RESISTANCE"],
                        row["BOX4_AIR_LEAKAGE_TEST_DETAIL"],
                        row["BOX4_AIR_LEAKAGE_TEST_RESULT"],
                        row["BOX4_TestTime"],//add QR code
                        row["Remark"]
                    );
                    if (row["Remark"].ToString() == "Doublicate")
                    {
                        dataGridView1.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Red;
                        dataGridView1.Rows[rowIndex].Cells["Remark"].Style.ForeColor = Color.Red; // Đổi màu chữ của cột "Remark" thành màu trắng
                    }
                }
              
                dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Descending);
            }

            //if (!string.IsNullOrEmpty(Qrcode.ToString().Trim()))
            //{

            ////using read excel / list
            //List<EXCELDATA> listdata = new List<EXCELDATA>();
            //listdata = Form1.list.Where(r=>r.TOPHOUSING.StartsWith(Qrcode.Trim())).ToList();

            //foreach (var data in listdata)
            //{
            //    dataGridView1.Rows.Add(data.TOPHOUSING, data.BOX1_GLUE_AMOUNT, data.BOX1_GLUE_DISCHARGE_VOLUME_VISION, data.INSULATOR_BAR_CODE, data.BOX1_GLUE_OVERFLOW_VISION, data.BOX1_HEATED_AIR_CURING, data.BOX2_GLUE_AMOUNT, data.BOX2_GLUE_DISCHARGE_VOLUME_VISION, data.FPCB_BAR_CODE, data.BOX2_GLUE_OVERFLOW_VISION, data.BOX2_HEATED_AIR_CURING, data.BOX3_DISTANCE, data.BOX3_GLUE_AMOUNT, data.BOX3_GLUE_DISCHARGE_VOLUME_VISION, data.BOX3_GLUE_OVERFLOW_VISION, data.BOX3_HEATED_AIR_CURING, data.BOX4_TIGHTNESS_AND_LOCATION_VISION, data.BOX4_HEIGHT_PARALLELISM, data.BOX4_RESISTANCE, data.BOX4_AIR_LEAKAGE_TEST_DETAIL, data.BOX4_AIR_LEAKAGE_TEST_RESULT, data.BOX4_TestTime);
            //    dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Descending);
            //}
            //if (listdata.Count > 1)
            //{
            //    dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);
            //}


            //}
            //else
            //{
            //    foreach (var data in Form1.list)
            //    { 
            //        dataGridView1.Rows.Add(data.TOPHOUSING,data.BOX1_GLUE_AMOUNT, data.BOX1_GLUE_DISCHARGE_VOLUME_VISION, data.INSULATOR_BAR_CODE, data.BOX1_GLUE_OVERFLOW_VISION, data.BOX1_HEATED_AIR_CURING, data.BOX2_GLUE_AMOUNT, data.BOX2_GLUE_DISCHARGE_VOLUME_VISION, data.FPCB_BAR_CODE, data.BOX2_GLUE_OVERFLOW_VISION, data.BOX2_HEATED_AIR_CURING, data.BOX3_DISTANCE, data.BOX3_GLUE_AMOUNT, data.BOX3_GLUE_DISCHARGE_VOLUME_VISION, data.BOX3_GLUE_OVERFLOW_VISION, data.BOX3_HEATED_AIR_CURING, data.BOX4_TIGHTNESS_AND_LOCATION_VISION, data.BOX4_HEIGHT_PARALLELISM, data.BOX4_RESISTANCE, data.BOX4_AIR_LEAKAGE_TEST_DETAIL, data.BOX4_AIR_LEAKAGE_TEST_RESULT, data.BOX4_TestTime);
            //        dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Descending);
            //    }
            //    if (Form1.list.Count > 1)
            //    {
            //        dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);
            //    }
            //}
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if ((e.ColumnIndex == 2 || e.ColumnIndex == 4 || e.ColumnIndex == 7 || e.ColumnIndex == 9 || e.ColumnIndex == 13 || e.ColumnIndex == 15 || e.ColumnIndex == 16 || e.ColumnIndex == 20) && e.Value != null)
            {
                string cellValue = e.Value.ToString().Trim();

                if (cellValue.ToUpper() == "NG")
                {
                    e.CellStyle.ForeColor = System.Drawing.Color.Red;
                }
                else if (cellValue.ToUpper() == "OK")
                {
                    e.CellStyle.ForeColor = System.Drawing.Color.Green;
                }
            }
        }
    }
}
