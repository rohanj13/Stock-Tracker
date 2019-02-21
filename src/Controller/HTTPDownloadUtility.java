package Controller;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.URL;
 

public class HTTPDownloadUtility {
	private static final String apiKey = "ENPUOE7N7F34OD9W";
	private static final int BUFFER_SIZE = 4096;
	private static final String fileURL = "https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol=MSFT&apikey=ENPUOE7N7F34OD9W&datatype=csv";
	private static final String saveDir = "src/";
	
	public static void downloadFile()
	            throws IOException {
	        URL url = new URL(fileURL);
	        HttpURLConnection httpConn = (HttpURLConnection) url.openConnection();
	        int responseCode = httpConn.getResponseCode();
	 
	        // always check HTTP response code first
	        if (responseCode == HttpURLConnection.HTTP_OK) {
//	            String fileName = "";
//	            String disposition = httpConn.getHeaderField("Content-Disposition");
//	            String contentType = httpConn.getContentType();
	            int contentLength = httpConn.getContentLength();
	            System.out.println("Content-Length = " + contentLength);
	            
	            InputStreamReader input = new InputStreamReader(httpConn.getInputStream());
	            BufferedReader buffer = null;
	            String line = "";
	            String csvSplitBy = ",";

	            try {

	                buffer = new BufferedReader(input);
	                while ((line = buffer.readLine()) != null) {
	                    String[] stockData = line.split(csvSplitBy);
	                    System.out.println(line);
	                    System.out.println(" [Symbol =" + stockData[0] + " , =" + stockData[1]);
	                }

	            } catch (FileNotFoundException e) {
	                e.printStackTrace();
	            } catch (IOException e) {
	                e.printStackTrace();
	            } finally {
	                if (buffer != null) {
	                    try {
	                        buffer.close();
	                    } catch (IOException e) {
	                        e.printStackTrace();
	                    }
	                }
	            }

	        
	         /*   if (disposition != null) {
	                // extracts file name from header field
	                int index = disposition.indexOf("filename=");
	                if (index > 0) {
	                    fileName = disposition.substring(index + 10,
	                            disposition.length() - 1);
	                }
	            } else {
	                // extracts file name from URL
	                fileName = fileURL.substring(fileURL.lastIndexOf("/") + 1,
	                        fileURL.length());
	            }*/
	 
	         //   System.out.println("Content-Type = " + contentType);
	           // System.out.println("Content-Disposition = " + disposition);
	           
	           // System.out.println("fileName = " + fileName);
	 
	            // opens input stream from the HTTP connection
	          //  InputStream inputStream = httpConn.getInputStream();
	            //String saveFilePath = saveDir + File.separator + fileName;
	             
	            // opens an output stream to save into file
	          //  FileOutputStream outputStream = new FileOutputStream(saveFilePath);
	 
//	            int bytesRead = -1;
//	            byte[] buffer = new byte[BUFFER_SIZE];
//	            while ((bytesRead = inputStream.read(buffer)) != -1) {
//	                outputStream.write(buffer, 0, bytesRead);
	            }
	 
	         //   outputStream.close();
	           // inputStream.close();
	 
	          //  System.out.println("File downloaded");
	      //  } else {
	       //     System.out.println("No file to download. Server replied HTTP code: " + responseCode);
	        //}
	        httpConn.disconnect();
	    }
	}


