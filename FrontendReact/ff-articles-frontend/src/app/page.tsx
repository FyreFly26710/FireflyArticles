"use client";
import "./index.css";
import MdEditor from "@/components/MdEditor";
import {useState} from "react";
import MdViewer from "@/components/MdViewer";


export default function HomePage() {
    const [text, setText] = useState<string>('');


    return (
        <div>
            <MdEditor value={text} onChange={setText}/>
            <MdViewer value={text}/>

            hello
        </div>
    )
        ;
}