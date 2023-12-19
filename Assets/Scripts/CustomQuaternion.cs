using UnityEngine;

public class CustomQuaternion
{
    //Zmienne
    public float x;
    public float y;
    public float z;
    public float w; //skalar

    const float AngleMultiplier = 0.001f; // stala do obliczenia kata obrotu 

    // Konstruktor domyslny (kwaternion jednostkowy (brak obrotu)
    public CustomQuaternion()
    {
        x = 0f;
        y = 0f;
        z = 0f;
        w = 1f; 
    }
    // Konstruktor z okreslonymi zmiennymi 
    public CustomQuaternion(float x, float y, float z, float w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }
    // Konstruktor z osia i katem w radianach
    public CustomQuaternion(Vector3 axis, float angle)
    {
        angle *= 0.5f;
        float sinHalf = Mathf.Sin(angle);
        x = sinHalf * axis.x;
        y = sinHalf * axis.y;
        z = sinHalf * axis.z;
        w = Mathf.Cos(angle);
    }
    // Metoda statyczna tworzaca kwaternion na podstawie osi i kata obrotu
    public static CustomQuaternion FromAxisAngle(Vector3 axis, float angle)
    {
        float halfAngle = angle * AngleMultiplier; // Obliczenie kata obrotu
        float sinHalf = Mathf.Sin(halfAngle); // Obliczenie sinusa kata
        return new CustomQuaternion(axis.x * sinHalf, axis.y * sinHalf, axis.z * sinHalf, Mathf.Cos(halfAngle)); // Utworzenie kwaternionu z osi i sinusa, kosinusa po³owy k¹ta
    }

    // Metoda statyczna wykonujaca mnozenie kwaternionow.
    public static CustomQuaternion Multiply(CustomQuaternion a, CustomQuaternion b)
    {
        // Obliczenia dla x, y, z, w
        float x = a.w * b.x + a.x * b.w + a.y * b.z - a.z * b.y;
        float y = a.w * b.y + a.y * b.w + a.z * b.x - a.x * b.z;
        float z = a.w * b.z + a.z * b.w + a.x * b.y - a.y * b.x;
        float w = a.w * b.w - a.x * b.x - a.y * b.y - a.z * b.z;

        return new CustomQuaternion(x, y, z, w); // Utworzenie nowego kwaternionu z wynikowych
    }

    // Metoda konwertujaca kwaternion na macierz 4x4
    public Matrix4x4 ToMatrix4x4()
    {
        // Obliczenia dla roznych elementow macierzy rotacji
        float xx = x * x;
        float xy = x * y;
        float xz = x * z;
        float xw = x * w;

        float yy = y * y;
        float yz = y * z;
        float yw = y * w;

        float zz = z * z;
        float zw = z * w;
        // Utworzenie nowej macierzy 4x4
        Matrix4x4 matrix = new Matrix4x4();

        // Przypisanie wartosci do poszczegolnych elementow macierzy rotacji
        matrix[0, 0] = 1 - 2 * (yy + zz);
        matrix[0, 1] = 2 * (xy - zw);
        matrix[0, 2] = 2 * (xz + yw);

        matrix[1, 0] = 2 * (xy + zw);
        matrix[1, 1] = 1 - 2 * (xx + zz);
        matrix[1, 2] = 2 * (yz - xw);

        matrix[2, 0] = 2 * (xz - yw);
        matrix[2, 1] = 2 * (yz + xw);
        matrix[2, 2] = 1 - 2 * (xx + yy);

        matrix[3, 3] = 1f;

        return matrix;
    }

    // Metoda obracaj¹ca wektor przy u¿yciu kwaternionu
    public Vector3 RotateVector(Vector3 vector)
    {
        Matrix4x4 rotationMatrix = ToMatrix4x4(); // Konwersja kwaternionu na macierz rotacji
        return rotationMatrix * vector; // Pomno¿enie macierzy rotacji przez wektor
    }

    public void Normalize()
    {
        float magnitude = Mathf.Sqrt(x * x + y * y + z * z + w * w);
        x /= magnitude;
        y /= magnitude;
        z /= magnitude;
        w /= magnitude;
    }
    // Metody zwracajace kierunki w przestrzeni wedlug aktualnego obrotu
    public Vector3 Forward()
    {
        return new Vector3(2 * (x * z + w * y), // skladowa x 
            2 * (y * z - w * x), // skladowa y
            1 - 2 * (x * x + y * y)); // skladowa z
    }

    public Vector3 Up()
    {
        return new Vector3(2 * (x * y - w * z), 1 - 2 * (x * x + z * z), 2 * (y * z + w * x));
    }

    public Vector3 Right()
    {
        return new Vector3(1 - 2 * (y * y + z * z), 2 * (x * y + w * z), 2 * (x * z - w * y));
    }

    public Vector3 Back()
    {
        return -Forward();
    }
}
