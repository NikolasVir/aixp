# AIXP v1.0.0 Requirements

**A**rtificial **I**ntelligence e**XP**lainer is a web app that lets users
upload a document and ask an AI chat bot about it

## Functional Requirements

### FR-01 User Actions

#### Must Have

1. The user shall be able to upload `pdf` documents
2. The user shall be able to upload one `pdf` document at a time
3. After the document uploads, the user shall be able to ask questions about it

### FR-02 Document Handling

#### Must Have

1. If the system cannot read the `pdf` document, it shall provide feedback to
   the user
2. The system shall be able to read scanned `pdf` documents, not strictly
   digital `pdf`s
3. The system shall state if the AI can't find an answer inside the document
4. The system shall validate if the provided file is a `pdf` file
5. The system shall display progress state during `pdf` upload and ai
   processing
6. The system shall support documents with a configurable maximum number of
   pages
7. The system shall support English and Greek languages

### FR-03 History and Accounts

#### Won't Have

1. The system shall provide user accounts
2. The system shall provide a chat history

## Non-Functional Requirements

### NFR-01 Security

#### Must Have

1. The system shall delete uploaded documents after the chat has been inactive
   (no new messages sent) after a configurable amount of time

### NFR-02 Accessibility

#### Must Have

1. The system shall have responsive UI
2. The system shall allow all interactive elements shall be accessible via
   keyboard
3. The system shall convey errors through text messages, not color alone

### NFR-03 Performance

#### Must Have

1. The system shall have a configurable maximum `pdf` size
2. The user submitted question shall have a configurable maximum character limit
