using System;
using Microsoft.Data.SqlClient;
using CardManagment;
using Dapper;
using System.Data.Common;
using Microsoft.IdentityModel.Tokens;


namespace SystemZarzadzaniaKartami
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string appTitle = Console.Title = "System zarządzania kartami";
            Console.WriteLine($"- {appTitle} -");

            // !Only for development to initialize local database and table (single use)
            //CardConnection devInit = new CardConnection("localhost", "CardsManagement"); 
            //devInit.DevInitDatabase();

            SystemManager systemManager = new SystemManager();
            systemManager.FunctionSelection(); //Main program loop

            Console.ReadLine();
        }
    }

    public class SystemManager
    {
        public enum OPTIONS
        {
            AddCard = 1,
            ReturnResult = 2,
            RemoveCard = 3,
        }

        public void FunctionSelection()
        {
            byte selection = 0;
            CardConnection dbConnection = new CardConnection("localhost", "CardsManagement");
            for (;;) 
            {
                Console.WriteLine();
                Console.WriteLine($@"Wybierz z listy poniżej interesującą Cię funkcję:
1. Zapisywanie karty
2. Wyszukiwanie karty
3. Usuwanie karty
Numer wybranej funkcji (1-3): ");
                try
                {
                    selection = Convert.ToByte(Console.ReadLine());
                    if (selection <= 0 || selection > 3) throw new Exception();
                    Console.WriteLine();
                }
                catch (Exception)
                {

                    Console.WriteLine("Bład podczas wybierania funkcji, wybierz porządane działanie podając cyfry z przedziału 1-3!");
                }

                switch (selection)
                {
                    case (byte)OPTIONS.AddCard:
                        Card newCard = new Card();

                        Console.WriteLine("Wybrano > Zapisywanie karty <");
                        for (;;)
                        {
                            Console.Write("Podaj numer konta: ");
                            newCard.OwnerId = Console.ReadLine();
                            if (newCard.OwnerId.IsInputValid())
                                break;
                            Console.Write("Numer konta nie może być pusty i może składać się tylko z cyfr..\n");
                        }
                        
                        for (;;)
                        {
                            Console.Write("Podaj pin do karty: ");
                            newCard.Pin = Console.ReadLine();
                            if (newCard.Pin.PinValidation())
                                break;
                            Console.WriteLine("Błedny PIN. PIN musi składać się z 4 cyfr (od 0 do 9).");
                        }

                        for (; ; )
                        {
                            Console.Write("Podaj numer seryjny karty: ");
                            newCard.CardSerialNumber = Console.ReadLine();
                            if (newCard.CardSerialNumber.IsInputValid())
                                break;
                            Console.Write("Number seryjny karty nie może być pusty i może składać się tylko z cyfr.\n");
                        }

                        var cardDetailsQuery = newCard.NewCardToSqlQuery();
                        if (dbConnection.ExecuteQuery(cardDetailsQuery) > 0)
                        {
                            Console.WriteLine();
                            Console.WriteLine($@"Szczegóły dodanej karty: 
>Numer konta właściciela: {newCard.OwnerId}
>PIN: {newCard.Pin}
>Numer seryjny karty: {newCard.CardSerialNumber}
>Unikalny identyfikator karty: {newCard.CardId}");
                        }
                        else Console.WriteLine("Nie udało się dodać karty, upewnij się że Numwer konta właściciela i numer seryjny karty nie znajdują się już w bazie.");
                        break;

                    case (byte)OPTIONS.ReturnResult:
                        Console.WriteLine("Wybrano > Wyszukiwanie karty <");
                        Console.Write("Podaj początek numeru konta/numeru karty/identyfikatora karty: ");
                        var query = Console.ReadLine().SearchTermToQuery();
                        Card cardResult = dbConnection.SelectQuery(query);
                        if (cardResult.OwnerId == null)
                        {
                            Console.WriteLine("Nie znaleziono karty o podanych szczegółach.");
                            break;
                        }
                        Console.WriteLine();
                        Console.WriteLine($@"Szczegóły znalezionej karty: 
>Numer konta właściciela: {cardResult.OwnerId}
>PIN: {cardResult.Pin}
>Numer seryjny karty: {cardResult.CardSerialNumber}
>Unikalny identyfikator karty: {cardResult.CardId}");
                        break;

                    case (byte)OPTIONS.RemoveCard:
                        Console.WriteLine("Wybrano > Usuwanie karty <");
                        Console.Write("Podaj numer seryjny karty do usunięcia: ");
                        var cardSerialnumber = Console.ReadLine();
                        Console.Write($"Podano seryjny numer karty: \"{cardSerialnumber}\". Czy napewno chcesz kontynuować czynność usunięcia?\n- Aby anulować, wciśnij dowolny przycisk na klawiaturze.\n- Kliknięcie [T/t] na klawiaturze zatwierdzi operację usunięcia.\nWciśnij odpowiedni przycisk do interesującej Cię akcji > ");
                        ConsoleKeyInfo cki = Console.ReadKey( );
                        if (cki.Key.ToString().ToLower() == "t")
                        {
                            if (dbConnection.ExecuteQuery(cardSerialnumber.RemoveCardToQuery()) > 0)
                                Console.WriteLine($"\n- Pomyślnie usunięto kartę z numerem seryjnym \"{cardSerialnumber}\".");
                            else
                            {
                                Console.WriteLine("\nNie znaleziono karty o takim numerze seryjnym.");
                                break;
                            }
                        }
                        else Console.WriteLine("\nAnulowano akcje usuwania karty.");
                        break;
                }
            }  
        }
    }
}