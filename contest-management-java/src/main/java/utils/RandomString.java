package utils;

import java.util.Random;

public class RandomString {
    public static String generateRandomString(int numChars) {
        Random rand = new Random();
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < numChars; i++)
            sb.append((char) ('a' + rand.nextInt(26)));
        return sb.toString();
    }
}
