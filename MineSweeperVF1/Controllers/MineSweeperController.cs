using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MineSweeperVF1.Models;
namespace MineSweeperVF1.Controllers
{
    public class MineSweeperController : Controller
    {
  
        public IActionResult Index()
        {
            return View();
        }

        private bool CheckBounds(List<List<Cell>> a, int y, int x)
        {
            if(x < 0 || y < 0)
            {
                return false;
            }

            if(y >= a.Count())
            {
                return false;
            }

            if(x >= a[y].Count())
            {
                return false;
            }
            return true;
        }

        private bool Finished(List<List<Cell>> Board)
        {
            int flagged = 0;
            int numBombs = 0;
            int hidden = 0;
            foreach (var y in Board)
            {
                foreach (var x in y) {
                    if (x.Flagged) flagged++;
                    if (x.isBomb) numBombs++;
                    if (x.Hidden) hidden++;
                }
            }
            if(flagged == numBombs && hidden == numBombs) { return true; }

            return false;
        }

        private int BombValue(List<List<Cell>> board, int y, int x)
        {
            int total = 0;
            //top left
            if (CheckBounds(board, y - 1, x - 1)) { if(board[y - 1][x - 1].isBomb) total++; }
            //top right
            if (CheckBounds(board, y - 1, x + 1)) { if (board[y - 1][x + 1].isBomb) total++; }
            //above
            if (CheckBounds(board, y-1, x)) { if (board[y-1][x].isBomb) total++; }
            //left
            if (CheckBounds(board, y, x-1)) { if (board[y][x-1].isBomb) total++; }
            //bottom right
            if (CheckBounds(board, y+1, x + 1)) { if (board[y + 1][x + 1].isBomb) total++; }
            //bottom left
            if (CheckBounds(board, y + 1, x - 1)) { if (board[y + 1][x - 1].isBomb) total++; }
            //right
            if (CheckBounds(board, y, x+1)) { if (board[y][x+1].isBomb) total++; }
            //bottom
            if (CheckBounds(board, y+1, x)) { if (board[y+1][x].isBomb) total++; }

            return total;
        }

        private List<Point> GetSurroundings(List<List<Cell>> board, Point point)
        {
            var list = new List<Point>();
            //above
            Point p;
            if(CheckBounds(board, point.Y+1, point.X))
            {
                p = new Point(point.Y+1, point.X);
                if (!board[p.Y][p.X].isBomb) { list.Add(new Point(p.Y, p.X)); }
            }

            //below
            if (CheckBounds(board, point.Y - 1, point.X))
            {
                p = new Point(point.Y - 1, point.X);
                if (!board[p.Y][p.X].isBomb) { list.Add(new Point(p.Y, p.X)); }
            }
            //left
            if(CheckBounds(board, point.Y, point.X - 1))
            {
                p = new Point(point.Y, point.X - 1);
                if (!board[p.Y][p.X].isBomb) { list.Add(new Point(p.Y, p.X)); }
            }

            //right
            if (CheckBounds(board, point.Y, point.X+1))
            {
                p = new Point(point.Y, point.X+1);
                if (!board[p.Y][p.X].isBomb) { list.Add(new Point(p.Y, p.X)); }
            }
            return list;
        }

        //acts like a flood fill
        private List<List<Cell>> Unhide(List<List<Cell>> board, Point point)
        {
            if (board[point.Y][point.X].Value != 0)
            {
                board[point.Y][point.X].Hidden = false;
                return board;
            }

            Point p_tmp = point;
            List<Point> surroundings;
            Stack<Point> stack = new Stack<Point>();
            stack.Push(p_tmp);

            do
            {

                if (stack.Count > 0)
                {
                    p_tmp = stack.Pop();
                }

                surroundings =  GetSurroundings(board, p_tmp);

                foreach (var i in surroundings)
                {
                    if (board[i.Y][i.X].Hidden && board[i.Y][i.X].Value > 0)
                    {
                        board[i.Y][i.X].Hidden = false;
                    }
                    if(board[i.Y][i.X].Value == 0 && board[i.Y][i.X].Hidden)
                    {
                        stack.Push(i);
                    }
                }

                board[p_tmp.Y][p_tmp.X].Hidden = false;


            } while (stack.Count > 0);

            return board;
        }

        [HttpPost]
        public IEnumerable<IEnumerable<Cell>> Click(List<List<Cell>> board, int x, int y)
        {
            if (Finished(board))
            {
                return new List<List<Cell>>();
            }
            if (board[y][x].isBomb)
            {
                return null;
            }
            board = Unhide(board, new Point(y, x));
            return board;
        }

        [EnableCors]
        public IEnumerable<IEnumerable<Cell>> GenerateBoard(int width, int height, int numBombs)
        {
            //prevent infinite loop
            if (width * height < numBombs) { return new List<List<Cell>>(); }

            var Board = new List<List<Cell>>();
           for(int i=0; i < height; i++)
            {
                Board.Add(new List<Cell>());
                for(int j=0; j < width; j++)
                {
                    Board[i].Add(new Cell());
                }
            }

            var gen = new Random();
            for( int i=0; i<numBombs; i++)
            {
                var rx = gen.Next(0, width);
                var ry = gen.Next(0, height);
                if (Board[ry][rx].isBomb)
                {
                    i--;
                    continue;
                }
                Board[ry][rx].isBomb = true;
            }

            for (int i = 0; i < Board.Count(); i++)
            {
                for (int j = 0; j < Board[i].Count(); j++)
                {
                    Board[i][j].Value = BombValue(Board, i, j);
                }
            }

            return Board;
        }
    }
}