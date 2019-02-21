package Controller;
	
import java.io.IOException;

import javafx.application.Application;
import javafx.fxml.FXMLLoader;
import javafx.stage.Stage;
import javafx.scene.Scene;
import javafx.scene.layout.BorderPane;
//import view.HomeScreen.fxml


public class Main extends Application {
	
	private Stage primaryStage;
	private BorderPane stockView;
	static HTTPDownloadUtility dl = new HTTPDownloadUtility();
	@Override
	public void start(Stage primaryStage) {
		try {
			primaryStage = this.primaryStage;
		} catch(Exception e) {
			e.printStackTrace();
		}
	}
	
	public void showStockView() {
		try {
			FXMLLoader loader = new FXMLLoader();
            loader.setLocation(Main.class.getResource("src/homeScreen.fxml"));
            stockView = (BorderPane) loader.load();
            
            // Show the scene containing the root layout.
            Scene scene = new Scene(stockView);
            primaryStage.setScene(scene);
            primaryStage.show();
		} catch(Exception e) {
			e.printStackTrace();
		}
	}
	
	public Stage getPrimaryStage() {
        return primaryStage;
    }

	
	public static void main(String[] args) {
		
		String apiKey = "ENPUOE7N7F34OD9W";
	    int timeout = 3000;
	    try {
			dl.downloadFile();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	    //showStockView();
		launch(args);
	}
}
