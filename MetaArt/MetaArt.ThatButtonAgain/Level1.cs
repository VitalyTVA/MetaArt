﻿namespace ThatButtonAgain;
class Level1 {
    /**
     * Mouse Functions. 
     * 
     * Click on the box and drag it across the screen. 
     */

    float bx;
    float by;
    int boxSize = 75;
    bool overBox = false;
    bool locked = false;
    float xOffset = 0;
    float yOffset = 0;

    void setup()
    {
        size(640, 360);
        bx = width / 2;
        by = height / 2;
        rectMode(RADIUS);
    }

    void draw()
    {
        background(0);

        // Test if the cursor is over the box 
        if (mouseX > bx - boxSize && mouseX < bx + boxSize &&
            mouseY > by - boxSize && mouseY < by + boxSize)
        {
            overBox = true;
            if (!locked)
            {
                stroke(255);
                fill(153);
            }
        }
        else
        {
            stroke(153);
            fill(153);
            overBox = false;
        }

        // Draw the box
        rect(bx, by, boxSize, boxSize);
    }

    void mousePressed()
    {
        if (overBox)
        {
            locked = true;
            fill(255, 255, 255);
        }
        else
        {
            locked = false;
        }
        xOffset = mouseX - bx;
        yOffset = mouseY - by;

    }

    void mouseDragged()
    {
        if (locked)
        {
            bx = mouseX - xOffset;
            by = mouseY - yOffset;
        }
    }

    void mouseReleased()
    {
        locked = false;
    }
}

public abstract class GameElement { }