
public class Java14 {
    record Author (int id, String name, String topic) {
        static int followers;

        public static String followerCount() {
            return "Followers are "+ followers;
        }

        public String description(){
            return "Author "+ name + " writes on "+ topic;
        }

    public Author{
            if (id < 0) {
                throw new IllegalArgumentException( "id must be greater than 0.");
            }
        }
    }

    public static void main(String[] args) {
        System.out.println("Current Java version is \"" + System.getProperty("java.version") + "\"");
        var author = new Author(1, "Name", "Topic");
        var str = author.description();
    }
}
