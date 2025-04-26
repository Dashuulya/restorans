using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace restoran
{
    /// <summary>
    /// Логика взаимодействия для Stolik.xaml
    /// </summary>
    public partial class Stolik : Window
    {
        private string connectionString = "Server=localhost;Database=restoran;Trusted_Connection=True;";
        private ObservableCollection<StolikModel> stoliks = new ObservableCollection<StolikModel>();
        public Stolik()
        {
            InitializeComponent();
            LoadStoliks(); 
        }
        private void LoadStoliks()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT Stolik, TableNumber, Capacity, IsActive FROM Stoliks";
                    using (var adapter = new SqlDataAdapter(query, connection))
                    {
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        stoliks.Clear();

                        foreach (DataRow row in dataTable.Rows)
                        {
                            stoliks.Add(new StolikModel
                            {
                                Stolik = Convert.ToInt32(row["Stolik"]),
                                TableNumber = row["TableNumber"].ToString(),
                                Capacity = Convert.ToInt32(row["Capacity"]),
                                IsActive = Convert.ToBoolean(row["IsActive"])
                            });
                        }

                        TablesDataGrid.ItemsSource = stoliks;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке столиков: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void StatusChanged(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.DataContext is StolikModel stolik)
            {
                try
                {
                    using (var connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "UPDATE Stoliks SET IsActive = @IsActive WHERE Stolik = @Stolik";
                        using (var command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@IsActive", stolik.IsActive);
                            command.Parameters.AddWithValue("@Stolik", stolik.Stolik);
                            command.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Статус столика успешно обновлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обновлении статуса столика: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void AddTable_Click(object sender, RoutedEventArgs e)
        {
            string tableNumber = TableNumberTextBox.Text.Trim();
            if (!int.TryParse(CapacityTextBox.Text, out int capacity) || string.IsNullOrEmpty(tableNumber))
            {
                MessageBox.Show("Пожалуйста, введите корректные данные.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO Stoliks (TableNumber, Capacity, IsActive) VALUES (@TableNumber, @Capacity, 1)";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@TableNumber", tableNumber);
                        command.Parameters.AddWithValue("@Capacity", capacity);
                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Столик успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadStoliks();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении столика: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var mainPage = new MainPage();
            mainPage.Show();
            this.Close();
        }
    }

    public class StolikModel
    {
        public int Stolik { get; set; } 
        public string TableNumber { get; set; } 
        public int Capacity { get; set; } 
        public bool IsActive { get; set; }
    }

 }

