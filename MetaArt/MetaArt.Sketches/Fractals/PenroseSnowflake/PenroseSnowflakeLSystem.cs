namespace Fractals;

class PenroseSnowflakeLSystem {
    readonly string axiom;
    readonly float startLength;
    readonly float theta;
    readonly string ruleF;

    int steps = 0;
    string production;
    float drawLength;
    int generations;

    public PenroseSnowflakeLSystem() {
        axiom = "F3-F3-F3-F3-F";
        ruleF = "F3-F3-F45-F++F3-F";
        startLength = 450.0f;
        theta = radians(18);
        production = axiom;
        drawLength = startLength;
        generations = 0;
    }

    public void simulate(int gen) {
        while(generations < gen) {
            production = iterate(production);
        }
    }

    public void render() {
        translate(width, height);
        int repeats = 1;

        steps += 3;
        if(steps > production.Length) {
            steps = production.Length;
        }

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


    string iterate(string prod_) {
        string newProduction = "";
        for(int i = 0; i < prod_.Length; i++) {
            char step = production[i];
            if(step == 'F') {
                newProduction = newProduction + ruleF;
            } else {
                if(step != 'F') {
                    newProduction = newProduction + step;
                }
            }
        }
        drawLength = drawLength * 0.4f;
        generations++;
        return newProduction;
    }
}


