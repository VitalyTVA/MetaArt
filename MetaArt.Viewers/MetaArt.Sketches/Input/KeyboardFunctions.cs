﻿namespace Input;
class KeyboardFunctions {
    /**
     * Keyboard Functions 
     * by Martin Gomez 
     * 
     * Click on the window to give it focus and press the letter keys to type colors. 
     * The keyboard function keyPressed() is called whenever
     * a key is pressed. keyReleased() is another keyboard
     * function that is called when a key is released.
     * 
     * Original 'Color Typewriter' concept by John Maeda. 
     */

    const int maxHeight = 40;
    const int minHeight = 20;
    int letterHeight = maxHeight; // Height of the letters
    const int letterWidth = 20;          // Width of the letter

    int x = -letterWidth;          // X position of the letters
    int y = 0;                      // Y position of the letters

    bool newletter;

    const int numChars = 26;      // There are 26 characters in the alphabet
    Color[] colors = new Color[numChars];

    void setup() {
        size(640, 360);
        noStroke();
        colorMode(HSB, numChars);
        // Set a hue value for each key
        for(int i = 0; i < numChars; i++) {
            colors[i] = color(i, numChars, numChars);
        }
    }

    void draw() {
        if(newletter == true) {
            // Draw the "letter"
            int y_pos;
            if(letterHeight == maxHeight) {
                y_pos = y;
                rect(x, y_pos, letterWidth, letterHeight);
            } else {
                y_pos = y + minHeight;
                rect(x, y_pos, letterWidth, letterHeight);
                fill(numChars / 2);
                rect(x, y_pos - minHeight, letterWidth, letterHeight);
            }
            newletter = false;
        }
    }

    void keyPressed() {
        // If the key is between 'A'(65) to 'Z' and 'a' to 'z'(122)
        if((key >= 'A' && key <= 'Z') || (key >= 'a' && key <= 'z')) {
            int keyIndex;
            if(key <= 'Z') {
                keyIndex = key - 'A';
                letterHeight = maxHeight;
                fill(colors[keyIndex]);
            } else {
                keyIndex = key - 'a';
                letterHeight = minHeight;
                fill(colors[keyIndex]);
            }
        } else {
            fill(0);
            letterHeight = 10;
        }

        newletter = true;

        // Update the "letter" position
        x = (x + letterWidth);

        // Wrap horizontally
        if(x > width - letterWidth) {
            x = 0;
            y += maxHeight;
        }

        // Wrap vertically
        if(y > height - letterHeight) {
            y = 0;      // reset y to 0
        }
    }
}
