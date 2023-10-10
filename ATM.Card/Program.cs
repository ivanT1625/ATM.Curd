using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Globalization;

namespace ATM.Card
{
   public class CardHolder
   {
      String cardNum;
      string firstName;
      string lastName;
      int pin;
      double balance;

      public CardHolder(string cardNum, int pin, string firstName, string lastName, double balance)
      {
         this.cardNum = cardNum;
         this.firstName = firstName;
         this.lastName = lastName;
         this.pin = pin;
         this.balance = balance;
      }

      public String CardNum { get { return cardNum; } set { cardNum = value; } }
      public int Pin { get { return pin; } set { pin = value; } }
      public String FirstName { get { return firstName; } set { firstName = value; } }
      public String LastName { get { return lastName; } set { lastName = value; } }
      public double Balance { get { return balance; } set { balance = value; } }

   }

   internal static class Program
   {
      private static SqlConnection sqlConnection = null;

      private static double CheckInputSum()
      {
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ",";
            bool result = false;
            double resultDeposit = 0;
            while (!result)
            {
               result = double.TryParse(Console.ReadLine(), out var deposit);
               if (result)
               {
                  resultDeposit = deposit;
                  break;
               }
               Console.WriteLine("Enter the value again");
            }
            return resultDeposit;

      }
      public static void PrintOptions()
         {
            Console.WriteLine("Please choose from one of the following options..."); // Пожалуйста выберите один из следующих вариантов
            Console.WriteLine("1. Deposit");
            Console.WriteLine("2. Withdraw"); // 2. Вывод
            Console.WriteLine("3. Show Balance");
            Console.WriteLine("4. Exit");
         }
      public static void Deposit(CardHolder currentUser)
         {
            Console.WriteLine("How mush $$ would you like to deposit ?");
            //double deposit = double.Parse(Console.ReadLine(), provider);
            currentUser.Balance += CheckInputSum();
            Console.WriteLine("Thank you for you $$. You new balance is: " + currentUser.Balance);
         }
      public static void Withdraw(CardHolder currentUser)
         {
            Console.WriteLine("How mush $$ would you like to withdraw: ");
            //double withdrawal = Double.Parse(Console.ReadLine());
            double withdrawal = CheckInputSum();

            if (currentUser.Balance < withdrawal)
               Console.WriteLine("Insufficient balance (");
            else
            {
               currentUser.Balance -= withdrawal;
               Console.WriteLine("You're good to go! Thank you )");
            }
         }
      public static void Balance(CardHolder currentUser)
         {
            Console.WriteLine("Current balance : " + currentUser.Balance);
         }


      static void Main(string[] args)
      {

         SqlDataReader dataReader = null;
         List<CardHolder> listCardHolder = new List<CardHolder>();
         try
         {
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["CardHoldersDB"].ConnectionString);
            sqlConnection.Open();

            if (sqlConnection.State != ConnectionState.Open)
               throw new Exception("Подключение к базе данных отсутствует!");

            SqlCommand sqlCommand = new SqlCommand("SELECT*FROM CardHolder", sqlConnection);
            dataReader = sqlCommand.ExecuteReader();

            while (dataReader.Read())
            {
               listCardHolder.Add(new CardHolder(Convert.ToString(dataReader.GetValue(1)), Convert.ToInt32(dataReader.GetValue(2)), Convert.ToString(dataReader.GetValue(3)), Convert.ToString(dataReader.GetValue(4)), Convert.ToDouble(dataReader.GetValue(5))));
            }
         }
         catch (Exception ex)
         {
            Console.WriteLine("Ошибка : " + ex.Message);
         }
         finally
         {
            if(dataReader != null && !dataReader.IsClosed)
               dataReader.Close();
         }

         Console.WriteLine("Welcome to SimpleATM");
         Console.WriteLine("Please insert your debit card: ");
         String? debitCardNum = "";
         CardHolder? currentUser;

         while (true)
         {
            try
            {
               debitCardNum = Console.ReadLine();
               currentUser = listCardHolder.FirstOrDefault(a => a.CardNum.Equals(debitCardNum));

               if (currentUser != null) break;
               else { Console.WriteLine("Сard not recognized. Please try again"); }
            }
            catch { Console.WriteLine("Сard not recognized. Please try again"); }
         }

         Console.WriteLine("Please enter your pin: ");
         int? pinUser;

         while (true)
         {
            try
            {
               pinUser = Int32.Parse(Console.ReadLine());
               if (currentUser.Pin == pinUser) break;
               else { Console.WriteLine("Incorrect pin. Please repeat pin"); }

            }
            catch { Console.WriteLine("Incorrect pin. Please try again"); }
         }

         Console.WriteLine("Welcome " + currentUser.FirstName + " :)");
         int option = 0;
         do
         {
            PrintOptions();
            try
            {
               option = int.Parse(Console.ReadLine());

            }
            catch { }

            if (option == 1) { Deposit(currentUser); }
            else if (option == 2) { Withdraw(currentUser); }
            else if (option == 3) { Balance(currentUser); }
            else if (option == 4) { break; }
            else { option = 0; }

         }
         while (option != 4);
         Console.WriteLine("Thank you! Have a nice day :)");
      }
   }
}