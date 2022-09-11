namespace Fractals;

class PenroseSnowflakeLSystem : LSystem {
    const string ruleF = "F3-F3-F45-F++F3-F";
    float theta = radians(18);

    public PenroseSnowflakeLSystem()
        : base(
            drawLength: 450.0f, 
            production: "F3-F3-F3-F3-F",
            scale: 0.4f
        ) {
    }

    protected override void renderCore(int steps) {
        translate(width, height);
        int repeats = 1;
        for(int i = 0; i < steps; i++) {
            char step = production[i];
            if(step == 'F') {
                for(int j = 0; j < repeats; j++) {
                    line(0, 0, 0, -drawLength);
                    translate(0, -drawLength);
                }
                repeats = 1;
            } else if(step == '+') {
                for(int j = 0; j < repeats; j++) {
                    rotate(theta);
                }
                repeats = 1;
            } else if(step == '-') {
                for(int j = 0; j < repeats; j++) {
                    rotate(-theta);
                }
                repeats = 1;
            } else if(step == '[') {
                pushMatrix();
            } else if(step == ']') {
                popMatrix();
            } else if((step >= 48) && (step <= 57)) {
                repeats += step - 48;
            }
        }
    }


    protected override string iterate() {
        string newProduction = "";
        for(int i = 0; i < production.Length; i++) {
            char step = production[i];
            if(step == 'F') {
                newProduction = newProduction + ruleF;
            } else {
                if(step != 'F') {
                    newProduction = newProduction + step;
                }
            }
        }
        return newProduction;
    }
}


