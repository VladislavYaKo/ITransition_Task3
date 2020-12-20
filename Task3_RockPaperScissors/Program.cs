using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Task3_RockPaperScissors
{
    class Program
    {
        enum Winer { Computer, User, None}
        class MoveInfo
        {
            public string name { get; set; }
            public List<string> winers { get; set; }
            public List<string> losers { get;set;}
            public MoveInfo() { }
            public MoveInfo(string name)
            {
                this.name = name;
            }
            public MoveInfo(string name, List<string> winers, List<string> losers)
            {
                this.name = name;
                this.winers = winers;
                this.losers = losers;
            }
        }
        static List<string> DefineWiners(string[] moves, int moveIndex)
        {
            int range = moves.Length / 2;
            List<string> result;
            if (moveIndex + range < moves.Length)
                result = moves.ToList().GetRange(moveIndex + 1, range);
            else
            {
                int loopedElemsCount = range - (moves.Length - (moveIndex + 1));
                result = moves.ToList().GetRange(moveIndex + 1, range - loopedElemsCount);
                result.AddRange(moves.ToList().GetRange(0, loopedElemsCount));
            }
            return result;
        }
        static List<string> DefineLosers(string[] moves, int moveIndex)
        {
            int range = moves.Length / 2;
            List<string> result;
            if (moveIndex - range > 0)
                result = moves.ToList().GetRange(moveIndex - range, range);
            else
            {
                int loopedElemsCount = Math.Abs(moveIndex - range);
                result = moves.ToList().GetRange(0, range - loopedElemsCount);
                result.AddRange(moves.ToList().GetRange(moves.Length - loopedElemsCount, loopedElemsCount));
            }
            return result;
        }
        static string[] FindRepeatedElems(string[] src)
        {
            List<string> repeated = new List<string>();
            List<string> checkedStrings = new List<string>();
            foreach (string s in src)
                if (checkedStrings.Contains(s))
                    if (!repeated.Contains(s))
                        repeated.Add(s);
                    else { }
                else
                    checkedStrings.Add(s);
            return repeated.ToArray();
        }
        static string IsCorrectInput(string[] moves)
        {
            string answer = "";
            if (moves.Length < 3)
                answer += "Moves amount should not be less than 3.\r\n";
            if ((moves.Length % 2) != 1)
                answer += "Moves amount should be odd. For example, rock paper scissors lizard Spock\r\n";
            string[] repeated = FindRepeatedElems(moves);
            if (repeated.Length > 0)
                answer += "Moves should not repeat. Your repeated moves:" + String.Join(" ", repeated) + "\r\n";
            return answer;
        }
        static byte[] CreateRandomKey()
        {
            byte[] key = new byte[16];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(key);
            }
            return key;
        }
        static string ChooseMove(string[] moves)
        {
            Random rnd = new Random();
            return moves[rnd.Next(0, moves.Length)];
        }
        static byte[] GetMoveHash(string move, byte[] key)
        {
            using (HMACSHA256 hmac = new HMACSHA256(key))
            {
                return hmac.ComputeHash(Encoding.UTF8.GetBytes(move));
            }
        }
        static void ShowAvailableMoves(string[] moves)
        {
            Console.WriteLine("Available moves:");
            for(int i = 0; i < moves.Length; i++)
            {
                Console.WriteLine("{0} - {1}", i+1, moves[i]);
            }
            Console.WriteLine("Choose your move: ");
            return;
        }
        static Winer DefineWiner(MoveInfo compMoveInfo, MoveInfo userMoveInfo)
        {
            if (compMoveInfo.name == userMoveInfo.name)
                return Winer.None;
            else if (compMoveInfo.winers.Contains(userMoveInfo.name))
                return Winer.User;
            else
                return Winer.Computer;
        }
        static void Main(string[] args)
        {
            string[] moves = args;
            string inputCheckAns = IsCorrectInput(moves);
            if (inputCheckAns.Length > 0)
            {
                Console.Write(inputCheckAns);
                return;
            }
            //List<MoveInfo> movesRelations = DefineRelations(moves);
            byte[] key = CreateRandomKey();
            string computerMove = ChooseMove(moves);
            byte[] hmac = GetMoveHash(computerMove, key);
            MoveInfo computerMoveInfo = new MoveInfo(computerMove, 
                DefineWiners(moves, Array.IndexOf(moves, computerMove)), DefineLosers(moves, Array.IndexOf(moves, computerMove)));
            Console.WriteLine("HMAC:\r\n{0}", BitConverter.ToString(hmac).Replace("-", ""));
            string userMove = "";
            while(true)
            {
                ShowAvailableMoves(moves);
                userMove = Console.ReadLine();
                try
                {
                    int ind = Int32.Parse(userMove);
                    userMove = moves[ind-1];
                    break;
                }catch(Exception)
                {
                    Console.WriteLine("No such available move. Choose again.");
                }
            }
            MoveInfo userMoveInfo = new MoveInfo(userMove,
                DefineWiners(moves, Array.IndexOf(moves, userMove)), DefineLosers(moves, Array.IndexOf(moves, userMove)));
            Console.WriteLine("Your move: {0}", userMove);
            Console.WriteLine("Computer move: {0}", computerMove);
            Console.WriteLine("Winer: {0}!", Enum.GetName(typeof(Winer), DefineWiner(computerMoveInfo, userMoveInfo)));
            Console.WriteLine("HMAC key:\r\n{0}", BitConverter.ToString(key).Replace("-", ""));
        }
    }
}
