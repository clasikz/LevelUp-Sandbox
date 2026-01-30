'use client';

import { useState } from 'react';
import Editor from '@monaco-editor/react';

export default function Home() {
    const [code, setCode] = useState('// Write your code here\nfunction solution() {\n    \n}');
    const [result, setResult] = useState('');
    const [loading, setLoading] = useState(false);

    const handleSubmit = async () => {
        setLoading(true);
        setResult('Evaluating...');
        
        try {
            const response = await fetch('https://levelupsandboxapi.onrender.com/api/evaluation/evaluate', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    exerciseId: '3fa85f64-5717-4562-b3fc-2c963f66afa6',
                    exerciseTitle: 'Reverse a String',
                    exerciseDescription: 'Write a function that reverses a string',
                    difficulty: 1,
                    userCode: code,
                    language: 'javascript'
                })
            });

            const data = await response.json();
            setResult(JSON.stringify(data, null, 2));
        } catch (error: any) {
            setResult('Error: ' + error.message);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="h-screen flex flex-col bg-black text-white">
            <header className="bg-gray-900 border-b border-gray-700 px-6 py-4">
                <h1 className="text-2xl font-bold">LevelUp Sandbox</h1>
            </header>

            <div className="flex-1 flex flex-col p-4 gap-4">
                <div className="flex-1 border border-gray-700 rounded-lg overflow-hidden">
                    <Editor
                        height="100%"
                        defaultLanguage="javascript"
                        theme="vs-dark"
                        value={code}
                        onChange={(value) => setCode(value || '')}
                        options={{
                            minimap: { enabled: false },
                            fontSize: 14,
                            scrollBeyondLastLine: false,
                        }}
                    />
                </div>

                <button 
                    onClick={handleSubmit}
                    disabled={loading}
                    className="bg-green-600 hover:bg-green-700 disabled:bg-gray-600 px-6 py-3 rounded font-medium"
                >
                    {loading ? 'Evaluating...' : 'Submit Code'}
                </button>

                <div className="h-64 bg-gray-900 border border-gray-700 rounded-lg p-4 overflow-auto">
                    <pre className="text-sm">{result || 'Results will appear here...'}</pre>
                </div>
            </div>
        </div>
    );
}