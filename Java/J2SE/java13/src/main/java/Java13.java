
public class Java13 {

	public static void main(String[] args) {
        System.out.println("Current Java version is \"" + System.getProperty("java.version") + "\"");

        final int day = (int)(Math.random()*10);
        final String attr = switch (day) {
            case 1, 3, 5, 7 -> {
                yield "odd";
            }
            case 2, 4, 6, 8 -> {
                yield "even";
            }
            default -> "invalid";
        };
        System.out.println(attr);
	}
}
