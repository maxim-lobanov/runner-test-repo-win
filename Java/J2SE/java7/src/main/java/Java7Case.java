public class Java7Case {

    public static void main(String[] args) {
        System.out.println("Current Java version is \"" + System.getProperty("java.version") + "\"");

        // Test
        final String input = "Java 7";

        final String JAVA5 = "Java 5";
        final String JAVA6 = "Java 6";
        final String JAVA7 = "Java 7";

        final String messageText = "Input is: ";

        switch (input) {
            case JAVA5:
                System.out.println(messageText + JAVA5);
                break;
            case JAVA6:
                System.out.println(messageText + JAVA6);
                break;
            case JAVA7:
                System.out.println(messageText + JAVA7);
                break;
        }

        System.out.println("Java 7 test succeeded");
    }
}
