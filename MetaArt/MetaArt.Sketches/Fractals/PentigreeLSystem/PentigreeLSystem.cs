namespace Fractals;

class PentigreeLSystem : LSystem {
    const string rule = "F-F++F+F-F-F";
    float theta = radians(72);
    public PentigreeLSystem() 
        : base(
            drawLength: 60.0f,  
            production: "F-F-F-F-F",
            scale: 0.6f
        ) {
    }

    protected override void renderCore(int steps) {
        translate(width / 4, height / 2);
        for(int i = 0; i < steps; i++) {
            char step = production[i];
            if(step == 'F') {
                noFill();
                stroke(255);
                line(0, 0, 0, -drawLength);
                translate(0, -drawLength);
            } else if(step == '+') {
                rotate(theta);
            } else if(step == '-') {
                rotate(-theta);
            } else if(step == '[') {
                pushMatrix();
            } else if(step == ']') {
                popMatrix();
            }
        }
    }

    protected override string iterate() {
        return production.Replace("F", rule);
    }

}


