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

namespace restoran
{
    /// <summary>
    /// Логика взаимодействия для OrdersWindow.xaml
    /// </summary>
    public partial class OrdersWindow : Window
    {
        private string connectionString = "Server=localhost;Database=restoran;Trusted_Connection=True;";
        public OrdersWindow()
        {
            InitializeComponent();
            LoadClients();
            LoadTables();
            LoadOrders();
        }

        // Загрузка клиентов для ComboBox
        private void LoadClients()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT ClientId, FirstName, LastName FROM Client";
                    using (var adapter = new SqlDataAdapter(query, connection))
                    {
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        // Создаем поле FullName для отображения в ComboBox
                        dataTable.Columns.Add("FullName", typeof(string), "LastName + ' ' + FirstName");

                        ClientComboBox.ItemsSource = dataTable.DefaultView;
                        ClientComboBox.DisplayMemberPath = "FullName";
                        ClientComboBox.SelectedValuePath = "ClientId";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке клиентов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Загрузка столиков для ComboBox
        private void LoadTables()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT Stolik, TableNumber FROM Stoliks";
                    using (var adapter = new SqlDataAdapter(query, connection))
                    {
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        TableComboBox.ItemsSource = dataTable.DefaultView;
                        TableComboBox.DisplayMemberPath = "TableNumber";
                        TableComboBox.SelectedValuePath = "Stolik";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке столиков: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Загрузка всех заказов
        private void LoadOrders()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT o.OrdersItemId, c.FirstName + ' ' + c.LastName AS ClientName, s.TableNumber, o.OrderDate
                        FROM OrdersItem o
                        INNER JOIN Client c ON o.ClientId = c.ClientId
                        INNER JOIN Stoliks s ON o.Stolik = s.Stolik";
                    using (var adapter = new SqlDataAdapter(query, connection))
                    {
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        OrdersDataGrid.ItemsSource = dataTable.DefaultView;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке заказов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Обработчик нажатия на кнопку "Добавить заказ"
        private void AddOrder_Click(object sender, RoutedEventArgs e)
        {
            if (ClientComboBox.SelectedValue == null || TableComboBox.SelectedValue == null)
            {
                MessageBox.Show("Выберите клиента и столик.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int clientId = (int)ClientComboBox.SelectedValue;
            int tableId = (int)TableComboBox.SelectedValue;

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO OrdersItem (ClientID, StolikID, time) VALUES (@ClientID, @StolikID, @time)";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ClientId", clientId);
                        command.Parameters.AddWithValue("@Stolik", tableId);
                        command.Parameters.AddWithValue("@time", DateTime.Now);
                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Заказ успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadOrders();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении заказа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
    
}
