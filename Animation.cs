using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Scarab_Commando
{
    class Animation:Sprite
    {
        // Número de frames na animação
        protected int frameCount;
        public int FrameCount
        { get { return frameCount; } set { frameCount = value; } }

        // Contador do tempo restante até o frame seguinte
        protected float timer = 0f;
        public float Timer
        { get { return timer; } set { timer = value; } }


        // Tempo entre dois frames da animação
        protected float interval = 100f;
        public float Interval
        { get { return interval; } set { interval = value; } }

     
        // Posição inicial da animação
        protected Vector2 initialPosition;
        public Vector2 InitialPosition
        {
            get { return initialPosition; }
            set { initialPosition = value; }
        }

        // Frame atual da animação
        protected int currentFrame = 0;
        public int CurrentFrame
        {
            get { return currentFrame; }
            set { currentFrame = value; }
        }

        // Largura de cada frame
        protected int width;
        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        // Altura de cada frame
        protected int height;
        public int Height
        {
            get { return height; }
            set { height = value; }
        }
        // Retângulo que percorrerá a imagem
        protected Rectangle SourceRect;
        // Retângulo aonde a animação será desenhada na tela
        protected Rectangle DestRect;

        
        private bool isOn, isGoing;
        
        // Ativada para preparar a animação
        public bool IsOn
        {
            get { return isOn; }
            set { isOn = value; }
        }

        // Mostra que a animação está ocorrendo
        public bool IsGoing
        {
            get { return isGoing; }
            set { isGoing = value; }
        }

        // Método da animação propriamente dita
        public void Animate(GameTime gameTime, Vector2 position)
        {
            // Diminui o tempo restante até o frame seguinte
            Timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            // Checa se o tempo passado é maior que o intervalo. Se sim, passa para o frame seguinte.
            if (Timer > Interval)
            {

                CurrentFrame++;

                // Se o frame for o último, volta para o primeiro frame, desativa a animação e zera a vibração do GamePad
                if (CurrentFrame > FrameCount - 1)
                {

                    CurrentFrame = 0;
                    IsGoing = false;
                    GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
                }
                // Zera o timer a cada frame trocado
                Timer = 0f;
            }



            // O retângulo percorre a imagem e usa a largura de cada frame e o número do frame atual para determinar qual será desenhado
           SourceRect = new Rectangle(CurrentFrame * Width, 0, Width, Height);

            // O retângulo aonde a animação será desenhada na tela
           DestRect = new Rectangle((int)position.X, (int)position.Y, Width, Height);

            
        }

        // Desenha a animação com base nos retângulos de fonte(SourceRect) e destino(DestRect) passados
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(mSpriteTexture, DestRect, SourceRect, Color.White);
        }
    }
}
