using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace restoran
{
    /// <summary>
    /// Логика взаимодействия для Clients.xaml
    /// </summary>
    public partial class Clients : Window
    {
        private string connectionString = "Server=localhost;Database=restoran;Trusted_Connection=True;";
        public Clients()
        {
            InitializeComponent();
            LoadClients();

        }

        private void Booking(object sender, RoutedEventArgs e)
        {
            string name = LastNameTextBox.Text.Trim();
            string famyly = FirstNameTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();
            string phone = PhoneTextBox.Text.Trim();
            string date = DateTextBox.Text.Trim();
            string capacity = CapacityTextBox.Text.Trim();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(famyly) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(date) || string.IsNullOrEmpty(capacity))
            {
                MessageBox.Show("Все поля обязательны для заполнения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!int.TryParse(phone, out int Phone) || !int.TryParse(capacity, out int Capacity))
            {
                MessageBox.Show("Поле 'Телефон' и количество гостей должно содержать только цифры.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!DateTime.TryParse(date, out DateTime reservationDate))
            {
                MessageBox.Show("Дата должна быть в корректном формате (например, 'YYYY-MM-DD').", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string checkQuery = "SELECT COUNT(*) FROM Client WHERE Email = @Email OR Phone = @Phone";
                    using (var checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@Email", email);
                        checkCommand.Parameters.AddWithValue("@Phone", phone);

                        int count = (int)checkCommand.ExecuteScalar();

                        if (count > 0)
                        {
                            MessageBox.Show("Клиент с таким email или телефоном уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }

                    string insertQuery = "INSERT INTO Client (FirstName, LastName, Email, Phone, ReservationDate, Capacity) VALUES (@FirstName, @LastName, @Email, @Phone, @ReservationDate, @Capacity);";
                    using (var command = new SqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@LastName", name);
                        command.Parameters.AddWithValue("@FirstName", famyly);
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@Phone", phone);
                        command.Parameters.AddWithValue("@ReservationDate", reservationDate);
                        command.Parameters.AddWithValue("@Capacity", capacity);
                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show($"Клиент успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                LastNameTextBox.Clear();
                FirstNameTextBox.Clear();
                EmailTextBox.Clear();
                PhoneTextBox.Clear();
                DateTextBox.Clear();
                CapacityTextBox.Clear();
                LoadClients();
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении клиента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void LoadClients()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string selectQuery = "SELECT ClientId,  FirstName, LastName, Email, Phone, ReservationDate, Capacity FROM Client;";
                    using (var adapter = new SqlDataAdapter(selectQuery, connection))
                    {
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        ClientsDataGrid.ItemsSource = dataTable.DefaultView; 
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке клиентов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var selectedRow = ClientsDataGrid.SelectedItem as DataRowView;
            if (selectedRow == null)
            {
                MessageBox.Show("Выберите клиента для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string clientName = selectedRow["FirstName"].ToString();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string deleteQuery = "DELETE FROM Client WHERE FirstName = @FirstName;";
                    using (var command = new SqlCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@FirstName", clientName);
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show($"Клиент '{clientName}' успешно удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show($"Клиент '{clientName}' не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }

                LoadClients();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении клиента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var mainPage = new MainPage();
            mainPage.Show();
            this.Close();
        }
    }
    
}
