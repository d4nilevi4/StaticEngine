namespace AtomSimulation;

// Samples points distributed as |Psi_nlm|^2 by inverse-transform sampling
// over discretized CDFs. r, theta, phi are independent because the density
// factors as [r^2 * R_nl^2] * [sin(theta) * P_l^m^2] * 1.
public sealed class OrbitalSampler
{
    const int NRadial = 4096;
    const int NPolar = 2048;

    readonly double[] _cdfR = new double[NRadial];
    readonly double[] _cdfTheta = new double[NPolar];

    int _cachedN = -1, _cachedL = -1, _cachedM = int.MinValue;
    double _rMax;

    public void EnsureCdfs(int n, int l, int m)
    {
        if (_cachedN == n && _cachedL == l && _cachedM == m) return;
        BuildRadial(n, l);
        BuildPolar(l, m);
        _cachedN = n;
        _cachedL = l;
        _cachedM = m;
    }

    // Tabulates and normalizes the radial CDF on [0, rMax]. The integrand
    // r^2 * R_nl(r)^2 is the radial probability density. rMax = 10*n^2*A0
    // covers the orbital tail since size scales as n^2.
    void BuildRadial(int n, int l)
    {
        _rMax = 10.0 * n * n * HydrogenWaveFunction.A0;
        double dr = _rMax / (NRadial - 1);
        double sum = 0.0;
        for (int i = 0; i < NRadial; ++i)
        {
            double r = i * dr;
            double R = HydrogenWaveFunction.RadialR(n, l, r);
            double pdf = r * r * R * R;
            sum += pdf;
            _cdfR[i] = sum;
        }
        if (sum > 0)
        {
            double inv = 1.0 / sum;
            for (int i = 0; i < NRadial; ++i) _cdfR[i] *= inv;
        }
    }

    // Polar CDF on [0, pi]. The sin(theta) factor is the spherical Jacobian;
    // without it the poles would be massively oversampled.
    void BuildPolar(int l, int m)
    {
        double dtheta = Math.PI / (NPolar - 1);
        double sum = 0.0;
        for (int i = 0; i < NPolar; ++i)
        {
            double theta = i * dtheta;
            double x = Math.Cos(theta);
            double Plm = HydrogenWaveFunction.AssociatedLegendre(l, m, x);
            double pdf = Math.Sin(theta) * Plm * Plm;
            sum += pdf;
            _cdfTheta[i] = sum;
        }
        if (sum > 0)
        {
            double inv = 1.0 / sum;
            for (int i = 0; i < NPolar; ++i) _cdfTheta[i] *= inv;
        }
    }

    // Inverse transform: u ~ U[0,1], find first CDF entry >= u, map back.
    public double SampleR(Random rng)
    {
        double u = rng.NextDouble();
        int idx = LowerBound(_cdfR, u);
        return idx * (_rMax / (NRadial - 1));
    }

    public double SampleTheta(Random rng)
    {
        double u = rng.NextDouble();
        int idx = LowerBound(_cdfTheta, u);
        return idx * (Math.PI / (NPolar - 1));
    }

    // Phi is uniform: |Psi|^2 has no phi dependence.
    public double SamplePhi(Random rng) => 2.0 * Math.PI * rng.NextDouble();

    static int LowerBound(double[] arr, double v)
    {
        int lo = 0, hi = arr.Length;
        while (lo < hi)
        {
            int mid = (lo + hi) >> 1;
            if (arr[mid] < v) lo = mid + 1;
            else hi = mid;
        }
        return Math.Min(lo, arr.Length - 1);
    }
}
