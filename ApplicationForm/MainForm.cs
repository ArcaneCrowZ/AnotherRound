﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace AnotherRound
{
    class MainForm : Form
    {
        Timer timer = new Timer();
        public Field Field { get; set; }
        public Controller Controller = new Controller();
        public Image Image;
        public int CurrentLevel;

        public void TryRestartGame()
        {
            var dialogResult = MessageBox.Show("Restart game?", "Conformation", MessageBoxButtons.OKCancel);
            if (dialogResult == DialogResult.OK)
            {
                RestartGame();
            }
            else
                Close();
        }

        public void RestartGame()
        {
            timer = new Timer();
            Field = new Field(CurrentLevel);
            Controller = new Controller();
            StartGame();
        }

        public void StartGame()
        {
            Field.EndGameEvent += TryRestartGame;
        }


        //Работа с формой
        #region
        /// <summary>
        /// Делает дела при загрузке классовой формы.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            DoubleBuffered = true;
            Text = "Another Round?";
            WindowState = FormWindowState.Maximized;
            StartGame();
        }

        /// <summary>
        /// Основной метод формы. Работает, пока работает форма.
        /// </summary>
        public MainForm(int level)
        {
            Field = new Field(level);
            CurrentLevel = level;

            Image = new Bitmap(Field.FieldSize.Width, Field.FieldSize.Height, PixelFormat.Format32bppArgb);

            timer.Interval = 10;
            timer.Tick += TimerTick;

            timer.Start();
        }

        /// <summary>
        /// Сборный метод для тика таймера игры.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void TimerTick(object sender, EventArgs args)
        {
            ExecuteContrloller();
            Invalidate();
        }
        #endregion

        //Отрисовка поля
        #region
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.FillRectangle(Brushes.SandyBrown, ClientRectangle);

            var g = Graphics.FromImage(Image);
            DrawField(g);

            e.Graphics.DrawImage(Image, 
                (ClientRectangle.Width - Image.Width) / 2, 
                (ClientRectangle.Height - Image.Height) / 2);
        }

        /// <summary>
        /// Заполняет игровое поле объектами.
        /// </summary>
        /// <param name="g">Заполняемый объект графики.</param>
        private void DrawField(Graphics g)
        {
            g.FillRectangle(Brushes.White, ClientRectangle);
            DrawObstacles(g);
            DrawPlayer(g);
            DrawProjectiles(g);
        }

        /// <summary>
        /// Прорисовывает все пули.
        /// </summary>
        /// <param name="g">Заполняемый объект графики.</param>
        private void DrawProjectiles(Graphics g)
        {
            foreach (var proj in Field.Projectails.Projectails)
                g.FillCentredEllipse(Brushes.Green, proj.Location, proj.Size);
        }
        /// <summary>
        /// Рисует игрока.
        /// </summary>
        /// <param name="g">Заполняемый объект графики.</param>
        private void DrawPlayer(Graphics g)
        {
            if (Field.Player.IsCanBeHited)
                g.FillCentredEllipse(Brushes.Blue, Field.Player.Location, Field.Player.Size);
            else
                g.FillCentredEllipse(Brushes.Yellow, Field.Player.Location, Field.Player.Size);
        }
        /// <summary>
        /// Рисует препятствия.
        /// </summary>
        /// <param name="g">Заполняемый объект графики.</param>
        private void DrawObstacles(Graphics g)
        {
            foreach (var obstacle in Field.ObjectsVault.GetAllObstacles())
            {
                var obstacleBrush = Brushes.Brown;
                var removableBrush = Brushes.Red;
                if (obstacle is CircleRemovable)
                    g.FillCentredEllipse(removableBrush, obstacle.Location, obstacle.Size);
                else if (obstacle is SquareRemovable)
                    g.FillCenterRectangle(removableBrush, obstacle.Location, obstacle.Size);
                else if (obstacle is ISquare)
                    g.FillCenterRectangle(obstacleBrush, obstacle.Location, obstacle.Size);
                else if (obstacle is ICircle)
                    g.FillCentredEllipse(obstacleBrush, obstacle.Location, obstacle.Size);
            }
        }
        #endregion

        //Работа с контроллером (вводом игрока)
        #region
        /// <summary>
        /// Вызывает класс контроллера для обработки введенных команд.
        /// </summary>
        private void ExecuteContrloller()
        {
            Controller.ExecuteContrloller(Field);
        }

        /// <summary>
        /// Ивенты при нажатии клавиши.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            Controller.HandleKey(e.KeyCode, true);
        }

        /// <summary>
        /// Ивенты при подъеме клавиши.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            Controller.HandleKey(e.KeyCode, false);
        }
        #endregion
    }

    //Методы расширения
    #region
    public static class GrapficsExtenshion
    {
        /// <summary>
        /// Рисует на форме овал с центром location размера size кистью brush.
        /// </summary>
        /// <param name="graphics">Заполняемый объект графики.</param>
        /// <param name="brush">Кисть из Brushes</param>
        /// <param name="location">Вектор-координаты центра овала.</param>
        /// <param name="size">Размеры овала по осям.</param>
        public static void FillCentredEllipse(this Graphics graphics, Brush brush, Vector location, Size size)
        {
            graphics.FillEllipse(brush, 
                location.X - size.Width / 2,
                location.Y - size.Height / 2,
                size.Width, size.Height);
        }

        /// <summary>
        /// Рисует на форме прямоугольник с центром location размера size кистью brush.
        /// </summary>
        /// <param name="graphics">Заполняемый объект графики.</param>
        /// <param name="brush">Кисть из Brushes</param>
        /// <param name="location">Вектор-координаты центра прямоугольника</param>
        /// <param name="size">Размер прямоугольника.</param>
        public static void FillCenterRectangle(this Graphics graphics, Brush brush, Vector location, Size size)
        {
            graphics.FillRectangle(brush,
                location.X - size.Width / 2,
                location.Y - size.Height / 2,
                size.Width, size.Height);
        }
    }
    #endregion
}