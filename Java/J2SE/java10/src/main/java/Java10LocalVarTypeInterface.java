import java.util.ArrayList;

public class Java10LocalVarTypeInterface {

    public static void main(String[] args) {
        System.out.println("Current Java version is \"" + System.getProperty("java.version") + "\"");

        // Test
        var list = new ArrayList<String>(); // infers ArrayList<String>
        System.out.println(list.toString());

        var stream = list.stream(); // infers Stream<String>
        System.out.println(stream.toString());

        System.out.println("Java 10 test succeeded");
    }
}