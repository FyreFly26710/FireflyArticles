import { Editor } from "@bytemd/react";
import gfm from "@bytemd/plugin-gfm";
import highlight from "@bytemd/plugin-highlight";
import "bytemd/dist/index.css";
import "highlight.js/styles/vs.css";
import 'github-markdown-css/github-markdown-light.css';
import styled from "styled-components";

interface Props {
    value?: string;
    onChange?: (v: string) => void;
    placeholder?: string;
}

const plugins = [gfm(), highlight()];

const EditorContainer = styled.div`
    .bytemd-toolbar-icon.bytemd-tippy.bytemd-tippy-right:nth-last-child(1),
    .bytemd-toolbar-icon.bytemd-tippy.bytemd-tippy-right:nth-last-child(2) {
        display: none;
    }

    .bytemd,
    .bytemd-body,
    .bytemd-preview {
        height: 100%;
        min-height: 300px;
    }
`;

const MdEditor = (props: Props) => {
    const { value = "", onChange, placeholder } = props;

    return (
        <EditorContainer>
            <Editor
                value={value}
                placeholder={placeholder}
                mode="split"
                plugins={plugins}
                onChange={onChange}
            />
        </EditorContainer>
    );
};

export default MdEditor; 