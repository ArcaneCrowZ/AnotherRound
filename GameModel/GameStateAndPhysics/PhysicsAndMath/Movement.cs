﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace AnotherRound
{
    public class Movement
    {
        public static Vector CalculateMoveVector(ICanMove movable, MapObjectsVault vault, Size fieldSize)
        {
            var moveVector = movable.GetMoveVector(vault);
            var withNewLocation = movable.GetCopyWithNewLocation(movable.Location + moveVector);


            if (movable.IsCanCollision)
                foreach (var obstacle in vault.GetAllObstacles())
                {
                    if (Physics.CollisionTwoAbstract(withNewLocation, obstacle))
                        moveVector = movable.ReactOnCollision(obstacle, moveVector);
                }

            return CheckMoveWithFieldBounds(moveVector, fieldSize, movable);
        }

        public static Vector CheckMoveWithFieldBounds(Vector move, Size fieldSize, ICanMove movable)
        {
            var newX = movable.Location.X + move.X;
            var newY = movable.Location.Y + move.Y;
            var moveX = move.X;
            var moveY = move.Y;

            if (newX > fieldSize.Width - movable.Size.Width / 2)
                moveX = (fieldSize.Width - movable.Size.Width / 2) - movable.Location.X;

            if (newX < movable.Size.Width / 2)
                moveX = movable.Location.X - movable.Size.Width / 2;

            if (newY > fieldSize.Height - movable.Size.Height / 2)
                moveY = (fieldSize.Height - movable.Size.Height / 2) - movable.Location.Y;

            if (newY < movable.Size.Height / 2)
                moveY = movable.Location.Y - movable.Size.Height / 2;

            return new Vector(moveX, moveY);
        }
    }
}