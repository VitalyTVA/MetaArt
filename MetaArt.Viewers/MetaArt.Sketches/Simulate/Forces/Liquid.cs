﻿namespace Simulate;
/**
 * Forces (Gravity and Fluid Resistence) with Vectors 
 * by Daniel Shiffman.  
 * 
 * Demonstration of multiple force acting on bodies (Mover class)
 * Bodies experience gravity continuously
 * Bodies experience fluid resistance when in "water"
 */

class Liquid {

    // Liquid is a rectangle
    float x, y, w, h;
    // Coefficient of drag
    float c;

    public Liquid(float x_, float y_, float w_, float h_, float c_) {
        x = x_;
        y = y_;
        w = w_;
        h = h_;
        c = c_;
    }

    // Is the Mover in the Liquid?
    public bool contains(Mover m) {
        PVector l = m.position;
        if(l.x > x && l.x < x + w && l.y > y && l.y < y + h) {
            return true;
        } else {
            return false;
        }
    }

    // Calculate drag force
    public PVector drag(Mover m) {
        // Magnitude is coefficient * speed squared
        float speed = m.velocity.mag();
        float dragMagnitude = c * speed * speed;

        // Direction is inverse of velocity
        PVector drag = m.velocity.copy();
        drag.mult(-1);

        // Scale according to magnitude
        drag.setMag(dragMagnitude);
        return drag;
    }

    public void display() {
        noStroke();
        fill(127);
        rect(x, y, w, h);
    }
}

