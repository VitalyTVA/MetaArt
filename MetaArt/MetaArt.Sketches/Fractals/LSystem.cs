namespace Fractals;
abstract class LSystem {
    readonly float scale;

    int steps = 0;
    int generations;

    protected float drawLength;
    protected string production { get; private set; }

    public LSystem(float drawLength, string production, float scale) {
        this.drawLength = drawLength;
        generations = 0;

        this.production = production;
        this.scale = scale;
    }

    public void simulate(int gen) {
        while(generations < gen) {
            production = iterate();
            generations++;
            drawLength = drawLength * scale;
        }
    }

    public void render(int advance) {
        steps += advance;
        if(steps > production.Length) {
            steps = production.Length;
        }
        renderCore(steps);
    }

    protected abstract void renderCore(int steps);
    protected abstract string iterate();
}


