namespace AtomSimulation;

// Radial and angular pieces of the hydrogen-like wave function.
// Psi_nlm = R_nl(r) * P_l^m(cos theta) * e^(i*m*phi); only magnitudes are used.
public static class HydrogenWaveFunction
{
    // Bohr radius in atomic units.
    public const double A0 = 1.0;

    public static double Factorial(int n)
    {
        double result = 1.0;
        for (int i = 2; i <= n; i++) result *= i;
        return result;
    }

    // Generalized Laguerre L_k^alpha(rho) via the three-term recurrence
    //   j * L_j = (2j - 1 + alpha - rho) * L_{j-1} - (j - 1 + alpha) * L_{j-2},
    // with seeds L_0 = 1, L_1 = 1 + alpha - rho. Stable up to large k.
    public static double AssociatedLaguerre(int k, double alpha, double rho)
    {
        if (k <= 0) return 1.0;
        double Lm1 = 1.0 + alpha - rho;
        if (k == 1) return Lm1;
        double Lm2 = 1.0;
        double L = 0.0;
        for (int j = 2; j <= k; ++j)
        {
            L = ((2 * j - 1 + alpha - rho) * Lm1 - (j - 1 + alpha) * Lm2) / j;
            Lm2 = Lm1;
            Lm1 = L;
        }

        return L;
    }

    // R_nl(r) = norm * exp(-rho/2) * rho^l * L_{n-l-1}^{2l+1}(rho),
    // rho = 2r / (n*A0). Norm makes integral of |R_nl|^2 * r^2 over r equal one.
    public static double RadialR(int n, int l, double r)
    {
        double rho = 2.0 * r / (n * A0);
        int k = n - l - 1;
        int alpha = 2 * l + 1;
        double L = AssociatedLaguerre(k, alpha, rho);
        double norm = Math.Pow(2.0 / (n * A0), 3) * Factorial(n - l - 1) / (2.0 * n * Factorial(n + l));
        return Math.Sqrt(norm) * Math.Exp(-rho / 2.0) * Math.Pow(rho, l) * L;
    }

    // Associated Legendre P_l^m(x). Built in two stages:
    //   1) Diagonal P_m^m = (-1)^m * (2m-1)!! * (1 - x^2)^(m/2).
    //   2) Step up in l with (l-m)*P_l^m = (2l-1)*x*P_{l-1}^m - (l+m-1)*P_{l-2}^m.
    public static double AssociatedLegendre(int l, int m, double x)
    {
        double Pmm = 1.0;
        if (m > 0)
        {
            double somx2 = Math.Sqrt((1.0 - x) * (1.0 + x));
            double fact = 1.0;
            for (int j = 1; j <= m; ++j)
            {
                Pmm *= -fact * somx2;
                fact += 2.0;
            }
        }

        if (l == m) return Pmm;
        double Pm1m = x * (2 * m + 1) * Pmm;
        if (l == m + 1) return Pm1m;
        for (int ll = m + 2; ll <= l; ++ll)
        {
            double Pll = ((2 * ll - 1) * x * Pm1m - (ll + m - 1) * Pmm) / (ll - m);
            Pmm = Pm1m;
            Pm1m = Pll;
        }

        return Pm1m;
    }
}