using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MultiQueueModels;
using MultiQueueTesting;
using System.IO;
using System.Diagnostics;
using System.Drawing.Design;
using System.Windows.Forms.DataVisualization.Charting;

namespace MultiQueueSimulation
{
    public partial class Form1 : Form
    {
        SimulationSystem system = new SimulationSystem();
        SimulationCase[] sim;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            comboBox1.Items.Clear();
            chart1.Series.Clear();
            system = new SimulationSystem();
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestCase1.txt");
            string[] lines = File.ReadAllLines(path);
            sim =  system.modelCalculation(system, lines);
            int coulmnsCount = 5 + 3 * system.NumberOfServers;
            dataGridView1.Columns.Add("CustomerNumber", "CustomerNumber");
            dataGridView1.Columns.Add("RandomInterArrival", "RandomInterArrival");
            dataGridView1.Columns.Add("InterArrival", "InterArrival");
            dataGridView1.Columns.Add("ArrivalTime", "ArrivalTime");
            dataGridView1.Columns.Add("RandomService", "RandomService");
            for (int i = 0; i < system.NumberOfServers; i++)
            {
                dataGridView1.Columns.Add("Server"+Convert.ToString(i+1) + "StartTime", "Server" + Convert.ToString(i + 1) + "StartTime");
                dataGridView1.Columns.Add("Server" + Convert.ToString(i+1) + "ServiceTime", "Server" + Convert.ToString(i + 1) + "ServiceTime");
                dataGridView1.Columns.Add("Server" + Convert.ToString(i + 1) + "EndTime", "Server" + Convert.ToString(i + 1) + "EndTime");
            }
            dataGridView1.Columns.Add("TimeInQueue", "TimeInQueue");
            dataGridView1.Rows.Add(100);
            for (int i = 0; i < system.StoppingNumber; i++)
            {
                dataGridView1.Rows[i].Cells[0].Value = sim[i+1].CustomerNumber;
                dataGridView1.Rows[i].Cells[1].Value = sim[i+1].RandomInterArrival;
                dataGridView1.Rows[i].Cells[2].Value = sim[i+1].InterArrival;
                dataGridView1.Rows[i].Cells[3].Value = sim[i+1].ArrivalTime;
                dataGridView1.Rows[i].Cells[4].Value = sim[i+1].RandomService;
                for (int j = 5, numberOftheServer = 0; j < coulmnsCount; numberOftheServer++, j = j + 3)
                {
                    if ((sim[i + 1].AssignedServer.ID-1) == numberOftheServer)
                    {
                        dataGridView1.Rows[i].Cells[j].Value = sim[i+1].StartTime;
                        dataGridView1.Rows[i].Cells[j + 1].Value = sim[i + 1].ServiceTime;
                        dataGridView1.Rows[i].Cells[j + 2].Value = sim[i + 1].EndTime;
                    }
                    else
                    {
                        dataGridView1.Rows[i].Cells[j].Value = " ";
                        dataGridView1.Rows[i].Cells[j+1].Value = " ";
                        dataGridView1.Rows[i].Cells[j+2].Value = " ";
                    }
                }
                dataGridView1.Rows[i].Cells[coulmnsCount].Value = sim[i + 1].TimeInQueue;

            }
            for (int i = 1; i <= system.StoppingNumber; i++)
            {
                system.SimulationTable.Add(sim[i]);
            }
            string result = TestingManager.Test(system, Constants.FileNames.TestCase1);
            MessageBox.Show(result);
            Application.EnableVisualStyles();
            for (int i = 0; i < system.NumberOfServers; i++)
            {
                comboBox1.Items.Add(Convert.ToString(i+1));
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            comboBox1.Items.Clear();
            chart1.Series.Clear();
            system = new SimulationSystem();
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestCase2.txt");
            string[] lines = File.ReadAllLines(path);
            sim = system.modelCalculation(system, lines);
            int coulmnsCount = 5 + 3 * system.NumberOfServers;
            dataGridView1.Columns.Add("CustomerNumber", "CustomerNumber");
            dataGridView1.Columns.Add("RandomInterArrival", "RandomInterArrival");
            dataGridView1.Columns.Add("InterArrival", "InterArrival");
            dataGridView1.Columns.Add("ArrivalTime", "ArrivalTime");
            dataGridView1.Columns.Add("RandomService", "RandomService");
            for (int i = 0; i < system.NumberOfServers; i++)
            {
                dataGridView1.Columns.Add("Server" + Convert.ToString(i + 1) + "StartTime", "Server" + Convert.ToString(i + 1) + "StartTime");
                dataGridView1.Columns.Add("Server" + Convert.ToString(i + 1) + "ServiceTime", "Server" + Convert.ToString(i + 1) + "ServiceTime");
                dataGridView1.Columns.Add("Server" + Convert.ToString(i + 1) + "EndTime", "Server" + Convert.ToString(i + 1) + "EndTime");
            }
            dataGridView1.Columns.Add("TimeInQueue", "TimeInQueue");
            dataGridView1.Rows.Add(20);
            for (int i = 0; i < system.StoppingNumber; i++)
            {
                dataGridView1.Rows[i].Cells[0].Value = sim[i + 1].CustomerNumber;
                dataGridView1.Rows[i].Cells[1].Value = sim[i + 1].RandomInterArrival;
                dataGridView1.Rows[i].Cells[2].Value = sim[i + 1].InterArrival;
                dataGridView1.Rows[i].Cells[3].Value = sim[i + 1].ArrivalTime;
                dataGridView1.Rows[i].Cells[4].Value = sim[i + 1].RandomService;
                for (int j = 5, numberOftheServer = 0; j < coulmnsCount; numberOftheServer++, j = j + 3)
                {
                    if ((sim[i + 1].AssignedServer.ID -1)== numberOftheServer)
                    {
                        dataGridView1.Rows[i].Cells[j].Value = sim[i + 1].StartTime;
                        dataGridView1.Rows[i].Cells[j + 1].Value = sim[i + 1].ServiceTime;
                        dataGridView1.Rows[i].Cells[j + 2].Value = sim[i + 1].EndTime;
                    }
                    else
                    {
                        dataGridView1.Rows[i].Cells[j].Value = " ";
                        dataGridView1.Rows[i].Cells[j + 1].Value = " ";
                        dataGridView1.Rows[i].Cells[j + 2].Value = " ";
                    }
                }
                dataGridView1.Rows[i].Cells[coulmnsCount].Value = sim[i + 1].TimeInQueue;
            }
            for (int i = 1; i <= system.StoppingNumber; i++)
            {
                system.SimulationTable.Add(sim[i]);
            }
            string result = TestingManager.Test(system, Constants.FileNames.TestCase2);
            MessageBox.Show(result);
            Application.EnableVisualStyles();
            for (int i = 0; i < system.NumberOfServers; i++)
            {
                comboBox1.Items.Add(Convert.ToString(i + 1));
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            chart1.Series.Clear();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            chart1.Series.Clear();
            for (int i = 0; i < system.NumberOfServers; i++)
            {
                if (system.Servers[i].ID == comboBox1.SelectedIndex+1)
                {
                    for (int k = 1; k <= system.StoppingNumber; k++)
                    {
                        if (system.Servers[i].ID == sim[k].AssignedServer.ID )
                        {
                            chart1.Series.Add("new" + Convert.ToString(k));
                            chart1.Series["new" + Convert.ToString(k)].Color = Color.Green;
                            chart1.Series["new" + Convert.ToString(k)].ChartType = SeriesChartType.Area;
                            chart1.Series["new" + Convert.ToString(k)].Points.AddXY(sim[k].StartTime, 1);
                            chart1.Series["new" + Convert.ToString(k)].Points.AddXY(sim[k].EndTime, 1);
                        }
                    }
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
