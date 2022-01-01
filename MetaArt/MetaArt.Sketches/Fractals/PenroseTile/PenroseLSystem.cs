namespace Fractals;

class PenroseLSystem : LSystem {
    const string ruleW = "YF++ZF4-XF[-YF4-WF]++";
    const string ruleX = "+YF--ZF[3-WF--XF]+";
    const string ruleY = "-WF++XF[+++YF++ZF]-";
    const string ruleZ = "--YF++++WF[+ZF++++XF]--XF";
    float theta = radians(36);
    public PenroseLSystem()
        : base(
            drawLength: 460.0f, 
            production: "[X]++[X]++[X]++[X]++[X]",
            scale: 0.5f
        ) {
    }

    protected override void renderCore(int steps) {
        translate(width / 2, height / 2);
        int pushes = 0;
        int repeats = 1;
        for(int i = 0; i < steps; i++) {
            char step = production[i];
            if(step == 'F') {
                stroke(255, 60);
                for(int j = 0; j < repeats; j++) {
                    line(0, 0, 0, -drawLength);
                    noFill();
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
                pushes++;
                pushMatrix();
            } else if(step == ']') {
                popMatrix();
                pushes--;
            } else if((step >= 48) && (step <= 57)) {
                repeats = (int)step - 48;
            }
        }

        // Unpush if we need too
        while(pushes > 0) {
            popMatrix();
            pushes--;
        }
    }

    protected override string iterate() {
        string newProduction = "";
        for(int i = 0; i < production.Length; i++) {
            char step = production[i];
            if(step == 'W') {
                newProduction = newProduction + ruleW;
            } else if(step == 'X') {
                newProduction = newProduction + ruleX;
            } else if(step == 'Y') {
                newProduction = newProduction + ruleY;
            } else if(step == 'Z') {
                newProduction = newProduction + ruleZ;
            } else {
                if(step != 'F') {
                    newProduction = newProduction + step;
                }
            }
        }

        return newProduction;
    }

}



