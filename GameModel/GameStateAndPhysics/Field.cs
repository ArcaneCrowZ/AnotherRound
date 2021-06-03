﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AnotherRound
{
    public class Field
    {
        public delegate void GameEnded();
        public event GameEnded EndGameEvent;
        public Field(Vector playerStartPosition)
        {
            MapObjectsVault.Player = new Player(playerStartPosition, new Size(50, 50));
        }

        public Size FieldSize { get; private set; } = new Size(1200, 700);
        public Player Player { get => MapObjectsVault.Player; } 
        public ProjectileList Projectails { get; private set; } = new ProjectileList();
        public MapObjectsVault MapObjectsVault { get; private set; } = MapObjectsVault.GenerateTestLevel();

        public void GameTick(ControlCommand moveCommand, ControlCommand shootCommand)
        {
            ExecuteAllProjectiles();
            MapObjectsVault.ExecuteMoving(FieldSize);
            TryShoot(shootCommand);
            MovePlayer(moveCommand);

            if (Player.IsDead)
                EndGameEvent.Invoke();
        }

        /// <summary>
        /// Проверка - находится ли объект за полем хотя бы частично.
        /// </summary>
        /// <param name="newLocation"></param>
        /// <param name="objectSize"></param>
        /// <returns></returns>
        private bool IsOutOfField(Vector newLocation, Size objectSize)
        {
            return newLocation.X > FieldSize.Width - objectSize.Width / 2 ||
                newLocation.X < objectSize.Width / 2 ||
                newLocation.Y > FieldSize.Height - objectSize.Height / 2 ||
                newLocation.Y < objectSize.Height / 2;
        }

        //Методы вызова компонентов игры.
        #region
        //Стрельба
        #region
        /// <summary>
        /// Выполняет действие всех объектов-пуль на поле.
        /// </summary>
        private void ExecuteAllProjectiles()
        {
            Projectails.ExecuteAllProjectiles(FieldSize, MapObjectsVault);
        }

        private void TryShoot(ControlCommand shootCommand)
        {
            Projectails.TryShoot(shootCommand.X, shootCommand.Y, Player);
        }
        #endregion

        //Движение персонажа игрока, проверка на смерть.
        #region
        /// <summary>
        /// Пробует передвинуть объект игрока по вводу игрока. Проверяет игрока на окончание игры.
        /// </summary>
        /// <param name="controlX">Обработанный ввод по оси х</param>
        /// <param name="controlY">Обработанный ввод по оси y</param>
        private void MovePlayer(ControlCommand moveCommand)
        {
            Player.DoPlayerMove(moveCommand.X, moveCommand.Y, MapObjectsVault, FieldSize);
        }
        #endregion
        #endregion
    }
}