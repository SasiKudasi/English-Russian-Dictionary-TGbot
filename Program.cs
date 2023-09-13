using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Polling;
using Telegram.Bot.Exceptions;
using System.Threading;
using Telegram.Bot.Types.ReplyMarkups;

namespace tgbot
{

    internal class Program
    {
       
        static Tutor tutor = new Tutor();
        static WordStorage storage = new WordStorage();

        static TelegramBotClient Bot = new TelegramBotClient("6462476904:AAElPcodQl2MSSco0ENNI_1IOZVkDZ953QI");

        static Dictionary<long, string> lastWord = new Dictionary<long, string>();
        
        static  void Main(string[] args)
        {
          

            var cts = new CancellationTokenSource();

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }
            };

            Bot.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cancellationToken: cts.Token);



           


            Console.ReadLine();
           
        }

        private static  ReplyKeyboardMarkup   GetKeyboard()
        {
            var keyboard = new[]
             {
                 new[]
                 {
                    new KeyboardButton("Старт"),
                    new KeyboardButton("Начать")
                  }
                 
            };

            return new ReplyKeyboardMarkup(keyboard);

        }

        private static Task HandleErrorAsync(ITelegramBotClient Bot, Exception exception, CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegra API ERROR: \n " +
                $"{apiRequestException.ErrorCode}\n" +
                $" {apiRequestException.Message}",
                _ => exception.ToString()
            };
            Console.WriteLine(errorMessage);
            return Task.CompletedTask;
        }


        static string AddWord(String[] msgArr)
        {
            if (msgArr.Length != 2)
            {
                return "Неправильное колличество аргументов. их должно быть 2";
            }
            else
            {
                tutor.AdWord(msgArr[0].ToLower(), msgArr[1].ToLower());
                return "новое слово добавленно в словарь";
            }
        }

        private static string GetRandomWord(long chatID)
        {
            var text = tutor.GetRandomWord();
            if (lastWord.ContainsKey(chatID))
            {
                lastWord[chatID] = text;
            }
            else 
                lastWord.Add(chatID, text);
            return text;
        }


        private static async Task HandleUpdateAsync(ITelegramBotClient Bot, Update update, CancellationToken cancellationToken)
        {
            if(update.Type == Telegram.Bot.Types.Enums.UpdateType.Message &&  update.Message!.Type == Telegram.Bot.Types.Enums.MessageType.Text)
            {
                var chatId = update.Message.Chat.Id;
                var messageText = update.Message.Text;
                var firstName = update.Message.From.FirstName;
                var msgArgs = messageText.Split(' ');
                Message sendMessage;
                String text;
                switch (msgArgs[0])
                {
                    case "Старт":
                        text = $"Привет, {firstName}! Что бы начать пользоваться ботом, нажмите кнопуку \"Начать\"," +
                            $"что бы добавить новое слово, напишите\" Добавить \" слово на английском слово на русском, " +
                            $"например добавить hello привет\"\" ";
                        break;
                    case "Добавить":
                        text = AddWord(msgArgs);
                        break;
                    case "Начать":
                        text = GetRandomWord(chatId);
                        break;
                    default:
                        if (lastWord.ContainsKey((chatId)))
                        {

                            text = CheckWord(lastWord[chatId], msgArgs[0]);
                            var newWord = GetRandomWord((chatId));
                            text = $"{text}\r\nСледующее слово {newWord}";
                        }
                        else
                        {
                            text = $"Привет, {firstName}!";
                        }
                        break;
                }
               sendMessage = await Bot.SendTextMessageAsync(chatId, text, replyMarkup: GetKeyboard());
               
            }
        }

        private static string CheckWord(string eng, string rus)
        {
            if (tutor.CheckWord(eng.ToLower(), rus.ToLower()))               
                return "Правильно!";                
            else
            {
                var correctAnswer = tutor.Translate(eng.ToLower());
                return $"Неверно! Правильный ответ {correctAnswer}";
            }
        }
    }
}
