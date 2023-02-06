using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;

public class ScreenCaster : MonoBehaviour
{
    public int    m_castFrameRate = 25;
    private float m_castPeriod    = 0.04f;

    private const int port = 8010;

    //This must be the-same with SEND_COUNT on the client
    const int SEND_RECEIVE_COUNT = 15;

    // from my code private 
    Texture2D mTextureVideoReceive = null;

    int mWidth  = 1536;
    int mHeight = 1404;

    int mResizedWidth;
    int mResizedHeight;

    int mSizeInBytes = 0;

    IntPtr mPixelsPtr = IntPtr.Zero;

    float timer = 0.0f;
    
    RenderTexture mRenderTexture; //= new RenderTexture(targetX, targetY, 24);
    Texture2D mResizedTexture;

    /// <summary> 	
	/// TCPListener to listen for incomming TCP connection 	
	/// requests. 	
	/// </summary> 
    private TcpListener tcpListener; 

    private Thread     tcpListenerThread;
    private TcpClient  connectedTcpClient;

    byte[] frameBytesLength;

    void Awake()
    {
        mSizeInBytes = mWidth * mHeight * 4; // RGBA

        mTextureVideoReceive = new Texture2D(mWidth, mHeight, TextureFormat.RGBA32, false);

        mResizedWidth  = mWidth / 3;
        mResizedHeight = mHeight / 3;

        mRenderTexture = new RenderTexture(mResizedWidth, mResizedHeight, 24);
        mResizedTexture = new Texture2D(mResizedWidth, mResizedHeight);

        frameBytesLength = new byte[SEND_RECEIVE_COUNT];

        
            
    }

    private void Start()
    {
        if (m_castFrameRate > 0)
        {
            m_castPeriod = 1.0f / (float) m_castFrameRate;
        }
        else
        {
            m_castPeriod = 0.04f;
        }

        Application.runInBackground = true;

        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
        Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
       
        Debug.Log("********** Start() tcpListenerThread");

        tcpListenerThread = new Thread(new ThreadStart(ListenForIncommingRequests));
        tcpListenerThread.IsBackground = true;
        tcpListenerThread.Start();   
    }

    /// <summary> 	
	/// Runs in background TcpServerThread; Handles incomming TcpClient requests 	
	/// </summary> 	
	private void ListenForIncommingRequests()
    {
        try
        {		
            tcpListener = new TcpListener(IPAddress.Any, port); // able to receive all IP adress 
            tcpListener.Start();
            Debug.Log("******** Server is listening");
            
            while (true)
            {            
                connectedTcpClient = tcpListener.AcceptTcpClient();
                Debug.Log("******** Server is connected");
                lynx.LynxSXRAPI.EnableCopyCameraAnd3DImage(true);
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("******* SocketException " + socketException.ToString());
        }
    }
    
    //Converts the data size to byte array and put result to the fullBytes array
    void byteLengthToFrameByteArray(int byteLength, byte[] fullBytes)
    {
        //Clear old data
        Array.Clear(fullBytes, 0, fullBytes.Length);
        //Convert int to bytes
        byte[] bytesToSendCount = BitConverter.GetBytes(byteLength);
        //Copy result to fullBytes
        bytesToSendCount.CopyTo(fullBytes, 0);
    }

    //Converts the byte array to the data size and returns the result
    int frameByteArrayToByteLength(byte[] frameBytesLength)
    {
        int byteLength = BitConverter.ToInt32(frameBytesLength, 0);
        return byteLength;
    }

    void Update()
    {
        if (connectedTcpClient == null)
        {
            return;
        }

        timer += Time.deltaTime;

        if (timer > m_castPeriod) 
        {          
            lynx.LynxSXRAPI.GetCameraAnd3DImage(ref mPixelsPtr);

            ResizeFrame();

            byte[] pngBytes = mResizedTexture.EncodeToJPG(70);

            //Fill total byte length to send. Result is stored in frameBytesLength
            byteLengthToFrameByteArray(pngBytes.Length, frameBytesLength);

            NetworkStream stream = null;

            try
            {
                stream = connectedTcpClient.GetStream();
            }
            catch (Exception e)
            {
                // @Cedric :
                // If the tcp distant client disconnect, the first exception is lower in the write fonction stream.Write fonction but seems
                // to be impossible to catch. the solution is to catch after the connectedTcpClient.GetStream(); exception and launch the process 
                // of the deconnexion in the listener. 
                Debug.Log("******** Exception in connectedTcpClient.GetStream() : " + e.ToString());
                Debug.Log("******** So close the TcpClient ");
                CloseConnection();
                return;
            }

            try
            {
              
                // Get a stream object for writing. 			                 
                if (stream == null)
                {
                    Debug.Log("******** stream == null");
                    return;
                }

                if (stream.CanWrite)
                {                              
                    try
                    {
                       stream.Write(frameBytesLength, 0, frameBytesLength.Length); // @ced : this the write call that launch an exception if socket was closed in the client but it's impossible to catch properly this one. 
                     
                       //Debug.Log("Sent Image byte Length: " + frameBytesLength.Length);

                        //Send the image bytes
                        stream.Write(pngBytes, 0, pngBytes.Length);
                        //Debug.Log("Sending Image byte array data : " + pngBytes.Length);
                    }
                    catch (SocketException socketException)
                    {
                        Debug.Log("************ SocketException 1 : " + socketException);
                    }
                }
                else
                {
                    Debug.Log("***************** stream can't write");
                    return;
                }
            }
            catch (SocketException socketException)
            {
                
                Debug.Log("************ SocketException 2 : " + socketException);
            }

            timer = 0.0f;
        }  
    }

    void CloseConnection()
    {
        connectedTcpClient.Close();
        connectedTcpClient = null;

        // Stop to get frame in Sxr API
        lynx.LynxSXRAPI.EnableCopyCameraAnd3DImage(false);
    }


    void ResizeFrame()
    {       
        mTextureVideoReceive.LoadRawTextureData(mPixelsPtr, mSizeInBytes);
        mTextureVideoReceive.Apply(); // necessaire

        // resize now texture 
        RenderTexture.active = mRenderTexture;
        Graphics.Blit(mTextureVideoReceive, mRenderTexture);
        mResizedTexture.ReadPixels(new Rect(0, 0, mResizedWidth, mResizedHeight), 0, 0);     
        RenderTexture.active = null;        
    }

    // stop everything
    private void OnApplicationQuit()
    {
        if (tcpListener != null)
            tcpListener.Stop();
        
        if (connectedTcpClient !=null)
            connectedTcpClient.Close();
    }
}